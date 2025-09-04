using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("playerID")]
    public string playerID = "Player1";

    public PlayerStat stat;
    private Shield shield;
    public SpriteRenderer sprite;
    public Animator animator;

    [SerializeField] private Vector2 aimOffsetLocal = Vector2.zero; //미세 조정용

    [Header("Death / Respawn")]
    public Transform respawnPoint; //세이브포인트
    [SerializeField] private float respawnDelay = 1.0f;
    [SerializeField] private float respawnIFrames = 1.0f;
    public bool IsDead { get; private set; }
    private Coroutine _dieCo;

    [Header("Hurt / Frames")]
    [SerializeField] private float invincibleDuration = 2f; // 무적 시간
    [SerializeField] private float knockbackTiles = 1f; // X축 넉백거리
    [SerializeField] private float knockbackImpulsePerTile = 6f;
    private bool isInvincible;

    [SerializeField] private GameObject weaponHitboxGO; //공격범위 콜라이더 탐색
    [SerializeField] private float attackWindow = 0.1f; //몇초동안
    private Coroutine _atkWindowCo;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f; //바닥 반지름
    public LayerMask groundMask; //레이어 바닥 필터 구분

    [Header("Ladder Ground Settings")] //사다리 바닥 선택
    [SerializeField] private LayerMask ladderGroundMask;

    private readonly List<Collider2D> _ignoredGroundCols = new List<Collider2D>();

    [Header("Crouch")]
    [SerializeField] private BoxCollider2D bodyCol;
    [SerializeField] private float crouchHeight = 0.5f; //엎드릴때 높이
    public bool IsCrouching { get; private set; } //엎드리는중?

    public bool IsGrounded { get; private set; }

    public bool IsOnLadder { get; private set; } //사다리 안?
    public bool IsClimbing { get; private set; } //사다리 사용중?
    
    float defaultGravity; //중력값 저장용
    int groundLayer;

    public bool IsParryWindow { get; private set; }
    private Coroutine _parryCo;

    Rigidbody2D rb;
    Vector2 standSize, standOffset; //기본 사이즈
    Vector2 crouchSize, crouchOffset; //엎드릴때 사이즈

    private float _nextAttackTime = 0f;

    //UI 체력바에 현재 체력 전달
    public event Action<float> onPlayerHealthChange;

    // 사다리 시작 시, 몸과 겹치는 Ground만 골라 무시할 때 쓸 필터
    private ContactFilter2D _groundFilter;
    int _ladderGroundLayer;

    private void Start()
    {
        // 모든 Awake 끝난 뒤 HP가 0 이하면 바로 사망 루틴 진입
        if (stat && stat.currentHP <= 0)
            TryCheckDeath();
    }

    private void Reset()
    {
        if (!bodyCol) bodyCol = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = stat.gravityScale;
        defaultGravity = stat.gravityScale;

        groundLayer = LayerMask.NameToLayer("Ground");
        if (!bodyCol) bodyCol = GetComponent<BoxCollider2D>();
        CacheColliderSizes();

        // Ground 전용 ContactFilter2D
        _groundFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = groundMask,
            useTriggers = false
        };

        _ladderGroundLayer = LayerMask.NameToLayer("LadderGround");

        shield = GetComponent<Shield>();
    }

    public void OpenAttackWindow() //공격 호출
    {
        if (!weaponHitboxGO) //자동탐색
        {
            
            Debug.LogError("No Weapon Hitbox");
            /*foreach (var col in GetComponentsInChildren<BoxCollider2D>(true))
            {
                if (col.isTrigger && col.gameObject != gameObject) { weaponHitboxGO = col.gameObject; break; }
            }
            if (!weaponHitboxGO) return; //못찾으면 패스*/
        }
        if (_atkWindowCo != null) StopCoroutine(_atkWindowCo);
        _atkWindowCo = StartCoroutine(_AttackWindowCo());
    }

    private IEnumerator _AttackWindowCo()
    {
        // 히트박스가 달린 GameObject 자체를 끄는것 보다, 히트박스 안에 있는 콜라이더를 끄는게 더 낫다고 튜터님께서 알려주셨습니다!
        weaponHitboxGO.GetComponent<BoxCollider2D>().enabled = true; //히트박스 o
        yield return new WaitForSeconds(attackWindow);
        weaponHitboxGO.GetComponent<BoxCollider2D>().enabled = false; //히트박스 x
        _atkWindowCo = null;
    }

    private void OnValidate()
    {
        if (!bodyCol) bodyCol = GetComponent<BoxCollider2D>();
        if (bodyCol) CacheColliderSizes();
    }

    private void CacheColliderSizes()
    {
        standSize = bodyCol.size; //원래 키
        standOffset = bodyCol.offset;

        crouchSize = new Vector2(standSize.x, crouchHeight); //x는 유지

        float deltaY = (standSize.y - crouchHeight) * 0.5f;
        crouchOffset = new Vector2(standOffset.x, standOffset.y - deltaY);
        //발 위치는 고정
    }

    public void SetCrouch(bool on) //엎드리기 계산
    {
        if (IsClimbing) on = false;
        if (on == IsCrouching) return;
        ApplyCrouch(on);
    }

    void ApplyCrouch(bool on)
    {
        IsCrouching = on;
        if (on)
        {
            bodyCol.size = crouchSize;
            bodyCol.offset = crouchOffset;
        }
        else
        {
            bodyCol.size = standSize;
            bodyCol.offset = standOffset;
        }
    }

    private void FixedUpdate()
    {
        if (IsClimbing)
        {
            IsGrounded = false; //사다리에서는 지면은 항상 false
        }
        else
        {
            IsGrounded = Physics2D.OverlapCircle
                (groundCheck.position, groundCheckRadius, groundMask | ladderGroundMask) // LadderGround도 바닥으로 포함
                    != null;
        }
    }

    public void Move(float xInput, bool run) //이동 계산
    {
        float speed = (run ? stat.runSpeed : stat.walkSpeed) * xInput;
        if (IsClimbing) speed = 0f;
        rb.velocity = new Vector2(speed, rb.velocity.y);

        // 방향 갱신: 입력 있을 때만 flipX 변경
        if (Mathf.Abs(xInput) > 0.01f)
            sprite.flipX = xInput < 0f;
        // 입력이 0이면 sprite.flipX 유지 > 마지막 시선 유지

        if (animator) animator.SetFloat("Speed", Mathf.Abs(speed));
    }

    public void Jump() //점프 계산
    {
        if (IsClimbing) return; //사다리에서는 점프 금지
        if (!IsGrounded) return;

        Vector2 v = rb.velocity;
        v.y = stat.jumpVelocity;
        rb.velocity = v;

        if (animator) animator.SetTrigger("Jump");
    }

    public void StartClimb() //사다리 사용 시작
    {
        if (!IsOnLadder) return;
        IsClimbing = true;

        // Player <> LadderGround 전체 충돌 끄기
        Physics2D.IgnoreLayerCollision(gameObject.layer, _ladderGroundLayer, true);

        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
    }

    public void StopClimb() // 사다리 사용 종료
    {
        IsClimbing = false;
        rb.gravityScale = defaultGravity;

        // ⬇️ Player <> LadderGround 다시 켜기
        Physics2D.IgnoreLayerCollision(gameObject.layer, _ladderGroundLayer, false);
    }

    public void Climb(float yInput, float climbSpeed)
    {
        if (!IsClimbing) return;
        rb.velocity = new Vector2(0f, yInput * climbSpeed);
    }

    void ToggleGroundCollision(bool ignore)
    {
        if (groundLayer < 0) return;
        Physics2D.IgnoreLayerCollision(gameObject.layer, groundLayer, ignore);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            IsOnLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            IsOnLadder = false;
            if (IsClimbing) StopClimb(); // 나가면 중력 복구
        }
    }

    public bool TryConsumeAttackCooldown()
    {
        float interval = 1f / Mathf.Max(0.01f, (stat ? stat.attackSpeed : 1f));
        if (Time.time < _nextAttackTime) return false;
        _nextAttackTime = Time.time + interval;
        return true;
    }

    public void ReceiveMonsterCollision(Vector3 sourcePos)
        //몬스터와 충돌 처리
    {
        if (isInvincible) return;
        if (shield.IsShieldOn)
        {
            shield.UseShield();
            return;
        }

        ApplyHurt(2, sourcePos, ignoreDefense : true);
    }

    public void ReceiveMonsterAttack(int rawDamage, Vector3 sourcePos)
        //몬스터의 공격 처리
    {
        if (isInvincible) return;
        if (shield.IsShieldOn)
        {
            shield.UseShield();
            return;
        }
        ApplyHurt(rawDamage, sourcePos, ignoreDefense : false);
    }

    public void ApplyHeal(int healAmount)
        //체력 회복
    {
        stat.currentHP = Mathf.Min(stat.currentHP + healAmount, stat.maxHP);

        //UI에 체력 변화 이벤트 전달
        onPlayerHealthChange?.Invoke(stat.currentHP);

        //콘솔확인
        Debug.Log($"[PLAYER HEAL] +{healAmount} HP  => {stat.currentHP}/{stat.maxHP}");
    }

    private void ApplyHurt(int rawDamage, Vector3 sourcePos, bool ignoreDefense)
        //공통: 체력감소 > 맞는 모션 > 넉백 > 무적
    {
        int finalDamage = ignoreDefense
        ? rawDamage : (stat ? stat.ReduceDamage(rawDamage) : Mathf.Max(1, rawDamage));

        stat.currentHP = Mathf.Max(0, stat.currentHP - finalDamage); //HP 적용

        //UI에 체력 변화 이벤트 전달
        onPlayerHealthChange?.Invoke(stat.currentHP);

        if (stat.currentHP <= 0) //죽으면 넉백, 무적 x
        {
            TryCheckDeath();
            return;
        }

        var controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.TriggerHitAnim();

        DoKnockbackFrom(sourcePos);

        StartCoroutine(IFrames());

        //콘솔확인
        Debug.Log($"[PLAYER HIT] -{finalDamage} HP  => {stat.currentHP}/{stat.maxHP}");
    }

    private void DoKnockbackFrom(Vector3 sourcePos)
    {
        //현재 속도가 있으면 그 반대, 없으면 상대 위치 기준
        int moveDir = Mathf.Abs(rb.velocity.x) > 0.05f ? (rb.velocity.x > 0 ? 1 : -1)
                    : (transform.position.x < sourcePos.x ? 1 : -1);
        int knockDir = -moveDir; // 진행 반대

        float impulse = (stat ? stat.tileSize : 1f) * knockbackTiles * knockbackImpulsePerTile;
        rb.AddForce(new Vector2(knockDir * impulse, 0f), ForceMode2D.Impulse);
    }

    private IEnumerator IFrames() //무적 타이머
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    private void TryCheckDeath()
    {
        if (IsDead) return;
        if (stat.currentHP <= 0)
        {
            if (_dieCo != null) StopCoroutine(_dieCo);
            _dieCo = StartCoroutine(DieAndRespawn());
        }
    }

    private IEnumerator DieAndRespawn()
    {
        IsDead = true;

        // 컨트롤러 비활성
        var controller = GetComponent<PlayerController>();
        if (controller) controller.enabled = false;

        // 등반/엎드림 정리
        IsClimbing = false; IsOnLadder = false;
        SetCrouch(false);

        // 공격 히트박스가 있다면 "오브젝트는 켠 채" 콜라이더만 끄기
        foreach (var col in GetComponentsInChildren<Collider2D>(true))
        {
            if (col.isTrigger && col.gameObject != gameObject)
            {
                col.enabled = false;  // 콜라이더만 OFF
                col.gameObject.SetActive(true); // 오브젝트는 항상 ON 유지
            }
        }

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        if (animator) animator.SetTrigger("Death");

        UIManager.Instance.GetUI<ScreenFader>().FadeOut();

        yield return new WaitForSeconds(respawnDelay);

        // 리스폰 지점 결정
        Vector3 targetPos = transform.position;
        if (respawnPoint) targetPos = respawnPoint.position;
        else
        {
            if (MapManager.Instance.CurrentMapType == MapType.Boss)
            {
                MapManager.Instance.MoveToAnotherMap(MapType.Stage1, true);
            }
            else
            {
                var sp = GameObject.FindGameObjectWithTag("SavePoint");
                if (!sp) sp = GameObject.FindGameObjectWithTag("Respawn");
                if (sp) targetPos = sp.transform.position;
                UIManager.Instance.GetUI<ScreenFader>().FadeIn();
            }
        }
        transform.position = targetPos;

        // 스탯/상태 초기화
        stat.currentHP = stat.maxHP;
        rb.gravityScale = stat.gravityScale;
        ToggleGroundCollision(false);
        rb.simulated = true;
        onPlayerHealthChange?.Invoke(stat.currentHP);

        // 애니메이터 초기화
        if (animator)
        {
            animator.ResetTrigger("Death");
            animator.ResetTrigger("Hit");
            animator.Rebind();
            animator.Update(0f);
            animator.SetFloat("Speed", 0f);
        }

        // 컨트롤러 복구
        if (controller) controller.enabled = true;

        IsDead = false;
    }

    public void BeginParryWindow(float duration = 2f)
    {
        if (_parryCo != null) StopCoroutine(_parryCo);
        _parryCo = StartCoroutine(_ParryWindowCo(duration));
    }

    private IEnumerator _ParryWindowCo(float duration)
    {
        IsParryWindow = true;
        yield return new WaitForSeconds(duration);
        IsParryWindow = false;
        _parryCo = null;
    }

    public Vector2 GetAimPoint(float height01 = 1.2f) //0 = 발, 1 = 머리
    {
        // height01: 0.0 = 발끝, 0.5 = 몸 중심, 1.0 = 머리
        var b = bodyCol.bounds; // 월드 기준 충돌 박스
        float y = Mathf.Lerp(b.min.y, b.max.y, Mathf.Clamp01(height01));
        Vector2 center = new Vector2(b.center.x, y);
        return center + (Vector2)transform.TransformVector(aimOffsetLocal);
    }

    public bool IsLeft()
    {
        return sprite.flipX;
    }

    //사다리 시작 시: LadderGround 레이어만 무시
    private void IgnoreGroundUnderLadder()
    {
        RestoreIgnoredGround(); // 혹시 이전에 남아있을 수 있으니 초기화

        var hits = new List<Collider2D>(8);
        bodyCol.OverlapCollider(new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = ladderGroundMask, // LadderGround 전용 레이어만 필터링
            useTriggers = false
        }, hits);

        foreach (var col in hits)
        {
            if (!col || !col.enabled) continue;
            Physics2D.IgnoreCollision(bodyCol, col, true); // 충돌 무시
            _ignoredGroundCols.Add(col); // 나중에 복구할 수 있도록 저장
        }
    }

    // 사다리 끝날 때: 무시했던 LadderGround 충돌 복구
    private void RestoreIgnoredGround()
    {
        foreach (var col in _ignoredGroundCols)
        {
            if (!col) continue; // 이미 삭제된 경우 체크
            Physics2D.IgnoreCollision(bodyCol, col, false); // 다시 충돌 켜기
        }
        _ignoredGroundCols.Clear();
    }

}
