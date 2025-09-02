using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("playerID")]
    public string playerID = "Player1";

    public PlayerStat stat;
    public SpriteRenderer sprite;
    public Animator animator;

    [Header("Hurt / Frames")]
    [SerializeField] private float invincibleDuration = 2f; // 무적 시간
    [SerializeField] private float knockbackTiles = 1f; // X축 넉백거리
    [SerializeField] private float knockbackImpulsePerTile = 6f;
    private bool isInvincible;


    [Header("Attack")]
    [Tooltip("가로 범위")]
    public float WidthRange = 2f; //2칸
    [Tooltip("세로 범위")]
    public float HeightRange = 1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f; //바닥 반지름
    public LayerMask groundMask; //레이어 바닥 필터 구분

    [Header("Crouch")]
    [SerializeField] private BoxCollider2D bodyCol;
    [SerializeField] private float crouchHeight = 0.5f; //엎드릴때 높이
    public bool IsCrouching { get; private set; } //엎드리는중?

    public bool IsGrounded { get; private set; }

    public bool IsOnLadder { get; private set; } //사다리 안?
    public bool IsClimbing { get; private set; } //사다리 사용중?
    float defaultGravity; //중력값 저장용
    int groundLayer;

    Rigidbody2D rb;
    Vector2 standSize, standOffset; //기본 사이즈
    Vector2 crouchSize, crouchOffset; //엎드릴때 사이즈

    private float _nextAttackTime = 0f;

    private void Reset()
    {
        if (!bodyCol) bodyCol = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = stat.gravityScale;
        defaultGravity = stat.gravityScale; //사다리에서 나오면 다시 돌려줄 중력값

        groundLayer = LayerMask.NameToLayer("Ground");

        if (!bodyCol) bodyCol = GetComponent<BoxCollider2D>();
        CacheColliderSizes();
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
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
            //지면 여부 계산
        }
    }

    public void Move(float xInput, bool run) //이동 계산
    {
        float speed = (run ? stat.runSpeed : stat.walkSpeed) * xInput;
        if (IsClimbing) speed = 0f;
        rb.velocity = new Vector2(speed, rb.velocity.y);

        if (sprite) sprite.flipX = speed < 0;
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

    public void StartClimb()  // W/S 입력으로 등반 시작할 때 호출
    {
        if (!IsOnLadder) return;
        IsClimbing = true;
        ToggleGroundCollision(true); //바닥과 충돌 끄기
        rb.gravityScale = 0f;          // 떨어지지 않게
        rb.velocity = Vector2.zero;
    }

    public void Climb(float yInput, float climbSpeed)
    {
        if (!IsClimbing) return;
        rb.velocity = new Vector2(0f, yInput * climbSpeed);
    }

    public void StopClimb()   // 사다리 사용 끝
    {
        IsClimbing = false;
        rb.gravityScale = defaultGravity;

        ToggleGroundCollision(false); //바닥과 충돌 켜기
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

    public void AttackOnce()
    {
        float tile = stat ? stat.tileSize : 1f;
        float range = WidthRange * tile; //가로길이
        float height = HeightRange * tile; //새로길이

        int dir = (sprite && sprite.flipX) ? -1 : 1;

        //플레이어 앞면부터 범위만큼 뻗도록 중심 계산
        float bodyHalfW = bodyCol ? bodyCol.size.x * 0.5f : 0f;
        Vector2 center = (Vector2)transform.position + 
            new Vector2(dir * (bodyHalfW + range * 0.5f), bodyCol ? bodyCol.offset.y : 0f);
        Vector2 size = new Vector2(range, height);

        var hits = Physics2D.OverlapBoxAll(center, size, 0f); //범위 내 모든 콜라이더
        var hitRoots = new HashSet<Transform>(); //동일 대상 중복 방지

        int damage = stat ? stat.attack : 1; //PlayerStat.attack 값
        int hitCount = 0;

        foreach (var h in hits)
        {
            if (!h) continue;

            // 자기 자신 스킵
            if (h.attachedRigidbody && h.attachedRigidbody.transform == this.transform)
                continue;

            // Tag=Monster만 타격
            if (!h.CompareTag("Monster")) continue;

            Transform root = h.attachedRigidbody ? h.attachedRigidbody.transform : h.transform;
            if (hitRoots.Contains(root)) continue;
            hitRoots.Add(root);
            hitCount++;

            //공격 여부 확인
            Debug.Log($"[HIT] Target={root.name}, Damage={damage} (PlayerStat.attack={damage}), Time={Time.time:F2}");


            //// 데미지 전달
            //int damage = stat ? stat.attack : 1;
            //root.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        }

        if (hitCount == 0)
        {
            Debug.Log($"[HIT] No target. Range={WidthRange}x{HeightRange} tiles, Dir={(dir > 0 ? "Right" : "Left")}, Time={Time.time:F2}");
        }
    }

    public bool TryConsumeAttackCooldown()
    {
        float interval = 1f / Mathf.Max(0.01f, (stat ? stat.attackSpeed : 1f));
        if (Time.time < _nextAttackTime) return false; //쿨타운
        _nextAttackTime = Time.time + interval; //다음 공격 예약
        return true;
    }

    public void ReceiveMonsterCollision(Vector3 sourcePos)
        //몬스터와 충돌 처리
    {
        if (isInvincible) return;
        ApplyHurt(2, sourcePos, ignoreDefense : true);
    }

    public void ReceiveMonsterAttack(int rawDamage, Vector3 sourcePos)
        //몬스터의 공격 처리
    {
        if (isInvincible) return;
        ApplyHurt(rawDamage, sourcePos, ignoreDefense : false);
    }

    private void ApplyHurt(int rawDamage, Vector3 sourcePos, bool ignoreDefense)
        //공통: 체력감소 > 맞는 모션 > 넉백 > 무적
    {
        int finalDamage = ignoreDefense //최소 데미지 1
            ? rawDamage : (stat ? stat.ReduceDamage(rawDamage) : Mathf.Max(1, rawDamage));

        stat.currentHP = Mathf.Max(0, stat.currentHP - finalDamage); //HP 적용

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
}
