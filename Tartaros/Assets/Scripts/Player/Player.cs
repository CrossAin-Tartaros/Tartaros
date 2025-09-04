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
    public SpriteRenderer sprite;
    public Animator animator;

    [SerializeField] private Vector2 aimOffsetLocal = Vector2.zero; //�̼� ������

    [Header("Death / Respawn")]
    public Transform respawnPoint; //���̺�����Ʈ
    [SerializeField] private float respawnDelay = 1.0f;
    [SerializeField] private float respawnIFrames = 1.0f;
    public bool IsDead { get; private set; }
    private Coroutine _dieCo;

    [Header("Hurt / Frames")]
    [SerializeField] private float invincibleDuration = 2f; // ���� �ð�
    [SerializeField] private float knockbackTiles = 1f; // X�� �˹�Ÿ�
    [SerializeField] private float knockbackImpulsePerTile = 6f;
    private bool isInvincible;

    [SerializeField] private GameObject weaponHitboxGO; //���ݹ��� �ݶ��̴� Ž��
    [SerializeField] private float attackWindow = 0.1f; //���ʵ���
    private Coroutine _atkWindowCo;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f; //�ٴ� ������
    public LayerMask groundMask; //���̾� �ٴ� ���� ����

    [Header("Crouch")]
    [SerializeField] private BoxCollider2D bodyCol;
    [SerializeField] private float crouchHeight = 0.5f; //���帱�� ����
    public bool IsCrouching { get; private set; } //���帮����?

    public bool IsGrounded { get; private set; }

    public bool IsOnLadder { get; private set; } //��ٸ� ��?
    public bool IsClimbing { get; private set; } //��ٸ� �����?
    
    float defaultGravity; //�߷°� �����
    int groundLayer;

    public bool IsParryWindow { get; private set; }
    private Coroutine _parryCo;

    Rigidbody2D rb;
    Vector2 standSize, standOffset; //�⺻ ������
    Vector2 crouchSize, crouchOffset; //���帱�� ������

    private float _nextAttackTime = 0f;

    //UI ü�¹ٿ� ���� ü�� ����
    public event Action<float> onPlayerHealthChange;

    private readonly List<Collider2D> _ignoredGroundCols = new List<Collider2D>();

    // ��ٸ� ���� ��, ���� ��ġ�� Ground�� ��� ������ �� �� ����
    private ContactFilter2D _groundFilter;

    private void Start()
    {
        // ��� Awake ���� �� HP�� 0 ���ϸ� �ٷ� ��� ��ƾ ����
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

        // Ground ���� ContactFilter2D
        _groundFilter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = groundMask,
            useTriggers = false
        };
    }

    public void OpenAttackWindow() //���� ȣ��
    {
        if (!weaponHitboxGO) //�ڵ�Ž��
        {
            
            Debug.LogError("No Weapon Hitbox");
            /*foreach (var col in GetComponentsInChildren<BoxCollider2D>(true))
            {
                if (col.isTrigger && col.gameObject != gameObject) { weaponHitboxGO = col.gameObject; break; }
            }
            if (!weaponHitboxGO) return; //��ã���� �н�*/
        }
        if (_atkWindowCo != null) StopCoroutine(_atkWindowCo);
        _atkWindowCo = StartCoroutine(_AttackWindowCo());
    }

    private IEnumerator _AttackWindowCo()
    {
        // ��Ʈ�ڽ��� �޸� GameObject ��ü�� ���°� ����, ��Ʈ�ڽ� �ȿ� �ִ� �ݶ��̴��� ���°� �� ���ٰ� Ʃ�ʹԲ��� �˷��ּ̽��ϴ�!
        weaponHitboxGO.GetComponent<BoxCollider2D>().enabled = true; //��Ʈ�ڽ� o
        yield return new WaitForSeconds(attackWindow);
        weaponHitboxGO.GetComponent<BoxCollider2D>().enabled = false; //��Ʈ�ڽ� x
        _atkWindowCo = null;
    }

    private void OnValidate()
    {
        if (!bodyCol) bodyCol = GetComponent<BoxCollider2D>();
        if (bodyCol) CacheColliderSizes();
    }

    private void CacheColliderSizes()
    {
        standSize = bodyCol.size; //���� Ű
        standOffset = bodyCol.offset;

        crouchSize = new Vector2(standSize.x, crouchHeight); //x�� ����

        float deltaY = (standSize.y - crouchHeight) * 0.5f;
        crouchOffset = new Vector2(standOffset.x, standOffset.y - deltaY);
        //�� ��ġ�� ����
    }

    public void SetCrouch(bool on) //���帮�� ���
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
            IsGrounded = false; //��ٸ������� ������ �׻� false
        }
        else
        {
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
            //���� ���� ���
        }
    }

    public void Move(float xInput, bool run) //�̵� ���
    {
        float speed = (run ? stat.runSpeed : stat.walkSpeed) * xInput;
        if (IsClimbing) speed = 0f;
        rb.velocity = new Vector2(speed, rb.velocity.y);

        // ���� ����: �Է� ���� ���� flipX ����
        if (Mathf.Abs(xInput) > 0.01f)
            sprite.flipX = xInput < 0f;
        // �Է��� 0�̸� sprite.flipX ���� > ������ �ü� ����

        if (animator) animator.SetFloat("Speed", Mathf.Abs(speed));
    }

    public void Jump() //���� ���
    {
        if (IsClimbing) return; //��ٸ������� ���� ����
        if (!IsGrounded) return;

        Vector2 v = rb.velocity;
        v.y = stat.jumpVelocity;
        rb.velocity = v;

        if (animator) animator.SetTrigger("Jump");
    }

    public void StartClimb() //��ٸ� ��� ����
    {
        if (!IsOnLadder) return;
        IsClimbing = true;

        //��ü ���̾� ���� ����, ��ġ�� �ٴڸ� ���α�
        IgnoreGroundUnderLadder();

        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
    }

    public void Climb(float yInput, float climbSpeed)
    {
        if (!IsClimbing) return;
        rb.velocity = new Vector2(0f, yInput * climbSpeed);
    }

    public void StopClimb() //��ٸ� ��� ����
    {
        IsClimbing = false;
        rb.gravityScale = defaultGravity;

        //��� ���� �ٴڸ� �ٽ� �ѱ�
        RestoreIgnoredGround();
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
            if (IsClimbing) StopClimb(); // ������ �߷� ����
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
        //���Ϳ� �浹 ó��
    {
        if (isInvincible) return;
        ApplyHurt(2, sourcePos, ignoreDefense : true);
    }

    public void ReceiveMonsterAttack(int rawDamage, Vector3 sourcePos)
        //������ ���� ó��
    {
        if (isInvincible) return;
        ApplyHurt(rawDamage, sourcePos, ignoreDefense : false);
    }

    public void ApplyHeal(int healAmount)
        //ü�� ȸ��
    {
        stat.currentHP = Mathf.Min(stat.currentHP + healAmount, stat.maxHP);

        //UI�� ü�� ��ȭ �̺�Ʈ ����
        onPlayerHealthChange?.Invoke(stat.currentHP);

        //�ܼ�Ȯ��
        Debug.Log($"[PLAYER HEAL] +{healAmount} HP  => {stat.currentHP}/{stat.maxHP}");
    }

    private void ApplyHurt(int rawDamage, Vector3 sourcePos, bool ignoreDefense)
        //����: ü�°��� > �´� ��� > �˹� > ����
    {
        int finalDamage = ignoreDefense
        ? rawDamage : (stat ? stat.ReduceDamage(rawDamage) : Mathf.Max(1, rawDamage));

        stat.currentHP = Mathf.Max(0, stat.currentHP - finalDamage); //HP ����

        //UI�� ü�� ��ȭ �̺�Ʈ ����
        onPlayerHealthChange?.Invoke(stat.currentHP);

        if (stat.currentHP <= 0) //������ �˹�, ���� x
        {
            TryCheckDeath();
            return;
        }

        var controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.TriggerHitAnim();

        DoKnockbackFrom(sourcePos);

        StartCoroutine(IFrames());

        //�ܼ�Ȯ��
        Debug.Log($"[PLAYER HIT] -{finalDamage} HP  => {stat.currentHP}/{stat.maxHP}");
    }

    private void DoKnockbackFrom(Vector3 sourcePos)
    {
        //���� �ӵ��� ������ �� �ݴ�, ������ ��� ��ġ ����
        int moveDir = Mathf.Abs(rb.velocity.x) > 0.05f ? (rb.velocity.x > 0 ? 1 : -1)
                    : (transform.position.x < sourcePos.x ? 1 : -1);
        int knockDir = -moveDir; // ���� �ݴ�

        float impulse = (stat ? stat.tileSize : 1f) * knockbackTiles * knockbackImpulsePerTile;
        rb.AddForce(new Vector2(knockDir * impulse, 0f), ForceMode2D.Impulse);
    }

    private IEnumerator IFrames() //���� Ÿ�̸�
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

        // ��Ʈ�ѷ� ��Ȱ��
        var controller = GetComponent<PlayerController>();
        if (controller) controller.enabled = false;

        // ���/���帲 ����
        IsClimbing = false; IsOnLadder = false;
        SetCrouch(false);

        // ���� ��Ʈ�ڽ��� �ִٸ� "������Ʈ�� �� ä" �ݶ��̴��� ����
        foreach (var col in GetComponentsInChildren<Collider2D>(true))
        {
            if (col.isTrigger && col.gameObject != gameObject)
            {
                col.enabled = false;  // �ݶ��̴��� OFF
                col.gameObject.SetActive(true); // ������Ʈ�� �׻� ON ����
            }
        }

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        if (animator) animator.SetTrigger("Death");

        yield return new WaitForSeconds(respawnDelay);

        // ������ ���� ����
        Vector3 targetPos = transform.position;
        if (respawnPoint) targetPos = respawnPoint.position;
        else
        {
            var sp = GameObject.FindGameObjectWithTag("SavePoint");
            if (!sp) sp = GameObject.FindGameObjectWithTag("Respawn");
            if (sp) targetPos = sp.transform.position;
        }
        transform.position = targetPos;

        // ����/���� �ʱ�ȭ
        stat.currentHP = stat.maxHP;
        rb.gravityScale = stat.gravityScale;
        ToggleGroundCollision(false);
        rb.simulated = true;

        // �ִϸ����� �ʱ�ȭ
        if (animator)
        {
            animator.ResetTrigger("Death");
            animator.ResetTrigger("Hit");
            animator.Rebind();
            animator.Update(0f);
            animator.SetFloat("Speed", 0f);
        }

        // ��Ʈ�ѷ� ����
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

    public Vector2 GetAimPoint(float height01 = 1.2f) //0 = ��, 1 = �Ӹ�
    {
        // height01: 0.0 = �߳�, 0.5 = �� �߽�, 1.0 = �Ӹ�
        var b = bodyCol.bounds; // ���� ���� �浹 �ڽ�
        float y = Mathf.Lerp(b.min.y, b.max.y, Mathf.Clamp01(height01));
        Vector2 center = new Vector2(b.center.x, y);
        return center + (Vector2)transform.TransformVector(aimOffsetLocal);
    }

    public bool IsLeft()
    {
        return sprite.flipX;
    }

    // ��ٸ� ���� ��: ���� �÷��̾� �ݶ��̴��� ��ġ�� Ground �ݶ��̴��鸸 ���д�
    private void IgnoreGroundUnderLadder()
    {
        RestoreIgnoredGround();

        var hits = new List<Collider2D>(8);
        //���� ��ġ�� Ground �ݶ��̴� ����
        bodyCol.OverlapCollider(_groundFilter, hits);

        foreach (var col in hits)
        {
            if (!col || !col.enabled) continue;
            Physics2D.IgnoreCollision(bodyCol, col, true);
            _ignoredGroundCols.Add(col);
        }
    }

    // ��ٸ����� ������ ��: ��� ���� �͸� �ٽ� �Ҵ�
    private void RestoreIgnoredGround()
    {
        foreach (var col in _ignoredGroundCols)
        {
            if (!col) continue; // �ı����� �� ����
            Physics2D.IgnoreCollision(bodyCol, col, false);
        }
        _ignoredGroundCols.Clear();
    }

}
