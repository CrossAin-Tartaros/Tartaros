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
    
    public bool IsGrounded { get; private set; }
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = stat.gravityScale;
    }

    private void FixedUpdate()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
        //���� ���� ���
    }


    public void Move(float xInput, bool run) //�̵� ���
    {
        float speed = (run ? stat.runSpeed : stat.walkSpeed) * xInput;
        rb.velocity = new Vector2(speed, rb.velocity.y);

        if (sprite) sprite.flipX = speed < 0;
        if (animator) animator.SetFloat("Speed", Mathf.Abs(speed));
    }

    public void Jump() //���� ���
    {
        if (!IsGrounded) return;
        Vector2 v = rb.velocity;
        v.y = stat.jumpVelocity;
        rb.velocity = v;

        if (animator) animator.SetTrigger("Jump");
    }
}
