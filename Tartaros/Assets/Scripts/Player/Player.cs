using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("playerID")]
    public string playerID = "Player1";

    public PlayerStat stat;
    public SpriteRenderer sprite;
    public Animator animator;

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
}
