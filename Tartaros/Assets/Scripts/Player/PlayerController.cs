using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Player player; // �̵�/���� �� ���� ����
    [SerializeField] private PlayerAnimation anim; // �ִϸ����� ����

    [Header("Animation Blend")]
    [SerializeField] private float speedLerp = 8f;

    private float curSpeed; //���� Animator�� �� �ӵ�

    private void Reset()
    {
        if (!player) player = GetComponent<Player>();
        if (!anim) anim = GetComponent<PlayerAnimation>() ?? GetComponentInChildren<PlayerAnimation>();
    }

    private void Awake()
    {
        if (!player) player = GetComponent<Player>();
        if (!anim) anim = GetComponent<PlayerAnimation>() ?? GetComponentInChildren<PlayerAnimation>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal"); // �¿� �̵�
        float y = Input.GetAxisRaw("Vertical");
        bool run = Input.GetKey(KeyCode.LeftShift); // �޸���
        bool jump = Input.GetKeyDown(KeyCode.Space); // ����
        bool attack = Input.GetMouseButtonDown(0); // ����
        bool crouchHeld = Input.GetKey(KeyCode.S) && !player.IsClimbing; //���帮��

        player.Move(x, run); //player.cs���� ȣ��

        if (player.IsOnLadder && Mathf.Abs(y) > 0.01f && !player.IsClimbing)
            player.StartClimb(); //w,s Ű���� ����

        if (player.IsClimbing)
        {
            float climbSpeed = run ? player.stat.runSpeed : player.stat.walkSpeed;
            player.Climb(y, climbSpeed);

            if (!player.IsOnLadder)
                player.StopClimb();

            if (jump)
            {
                player.StopClimb();
                player.Jump();
                jump = false; // �Ʒ� �Ϲ� ���� �ߺ� ����
            }
        }

        anim.SetClimb(player.IsClimbing);
        anim.SetClimbSpeed01(player.IsClimbing ? Mathf.Abs(y) : 0f);

        player.SetCrouch(crouchHeld);
        anim.SetCrouch(crouchHeld);

        float moveInput01 = Mathf.Clamp01(Mathf.Abs(x));
        float targetSpeed01 = moveInput01 * (run ? 1f : 0.5f);
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed01, Time.deltaTime * speedLerp);

        if (targetSpeed01 == 0f && curSpeed < 0.05f) curSpeed = 0f;
        curSpeed = Mathf.Clamp01(curSpeed);
        anim.SetSpeed01(curSpeed);

        anim.SetCrouch(crouchHeld);

        if (jump && !player.IsClimbing)
        {
            player.Jump();
            anim.TriggerJump();
        }

        if (attack && !player.IsClimbing)
        {
            player.AttackOnce();
            anim.TriggerAttack();
        }

        // ������ �׽�Ʈ
        if (Input.GetKeyDown(KeyCode.H))
        {
            anim.TriggerHit();
        }
    }
}
