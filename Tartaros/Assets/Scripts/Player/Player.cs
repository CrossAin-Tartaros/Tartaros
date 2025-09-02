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
    [SerializeField] private float invincibleDuration = 2f; // ���� �ð�
    [SerializeField] private float knockbackTiles = 1f; // X�� �˹�Ÿ�
    [SerializeField] private float knockbackImpulsePerTile = 6f;
    private bool isInvincible;


    [Header("Attack")]
    [Tooltip("���� ����")]
    public float WidthRange = 2f; //2ĭ
    [Tooltip("���� ����")]
    public float HeightRange = 1f;

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

    Rigidbody2D rb;
    Vector2 standSize, standOffset; //�⺻ ������
    Vector2 crouchSize, crouchOffset; //���帱�� ������

    private float _nextAttackTime = 0f;

    private void Reset()
    {
        if (!bodyCol) bodyCol = GetComponent<BoxCollider2D>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = stat.gravityScale;
        defaultGravity = stat.gravityScale; //��ٸ����� ������ �ٽ� ������ �߷°�

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

        if (sprite) sprite.flipX = speed < 0;
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

    public void StartClimb()  // W/S �Է����� ��� ������ �� ȣ��
    {
        if (!IsOnLadder) return;
        IsClimbing = true;
        ToggleGroundCollision(true); //�ٴڰ� �浹 ����
        rb.gravityScale = 0f;          // �������� �ʰ�
        rb.velocity = Vector2.zero;
    }

    public void Climb(float yInput, float climbSpeed)
    {
        if (!IsClimbing) return;
        rb.velocity = new Vector2(0f, yInput * climbSpeed);
    }

    public void StopClimb()   // ��ٸ� ��� ��
    {
        IsClimbing = false;
        rb.gravityScale = defaultGravity;

        ToggleGroundCollision(false); //�ٴڰ� �浹 �ѱ�
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

    public void AttackOnce()
    {
        float tile = stat ? stat.tileSize : 1f;
        float range = WidthRange * tile; //���α���
        float height = HeightRange * tile; //���α���

        int dir = (sprite && sprite.flipX) ? -1 : 1;

        //�÷��̾� �ո���� ������ŭ ������ �߽� ���
        float bodyHalfW = bodyCol ? bodyCol.size.x * 0.5f : 0f;
        Vector2 center = (Vector2)transform.position + 
            new Vector2(dir * (bodyHalfW + range * 0.5f), bodyCol ? bodyCol.offset.y : 0f);
        Vector2 size = new Vector2(range, height);

        var hits = Physics2D.OverlapBoxAll(center, size, 0f); //���� �� ��� �ݶ��̴�
        var hitRoots = new HashSet<Transform>(); //���� ��� �ߺ� ����

        int damage = stat ? stat.attack : 1; //PlayerStat.attack ��
        int hitCount = 0;

        foreach (var h in hits)
        {
            if (!h) continue;

            // �ڱ� �ڽ� ��ŵ
            if (h.attachedRigidbody && h.attachedRigidbody.transform == this.transform)
                continue;

            // Tag=Monster�� Ÿ��
            if (!h.CompareTag("Monster")) continue;

            Transform root = h.attachedRigidbody ? h.attachedRigidbody.transform : h.transform;
            if (hitRoots.Contains(root)) continue;
            hitRoots.Add(root);
            hitCount++;

            //���� ���� Ȯ��
            Debug.Log($"[HIT] Target={root.name}, Damage={damage} (PlayerStat.attack={damage}), Time={Time.time:F2}");


            //// ������ ����
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
        if (Time.time < _nextAttackTime) return false; //��Ÿ��
        _nextAttackTime = Time.time + interval; //���� ���� ����
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

    private void ApplyHurt(int rawDamage, Vector3 sourcePos, bool ignoreDefense)
        //����: ü�°��� > �´� ��� > �˹� > ����
    {
        int finalDamage = ignoreDefense //�ּ� ������ 1
            ? rawDamage : (stat ? stat.ReduceDamage(rawDamage) : Mathf.Max(1, rawDamage));

        stat.currentHP = Mathf.Max(0, stat.currentHP - finalDamage); //HP ����

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
}
