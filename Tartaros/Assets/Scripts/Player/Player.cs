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
}
