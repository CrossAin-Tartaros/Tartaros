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
    private bool isSettingPanelOn;

    public void TriggerHitAnim()
    {
        anim.TriggerHit();
    }


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
        bool setting = Input.GetKey(KeyCode.Escape); // ȯ�漳��

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

        if (jump && !player.IsClimbing)
        {
            player.Jump();
            anim.TriggerJump();
        }

        if (attack && !player.IsClimbing)
        {
            if (player.TryConsumeAttackCooldown()) // ��ٿ� üũ
            {
                player.OpenAttackWindow();
                anim.TriggerAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Vector3 fakeMonsterPos = player.transform.position + Vector3.right * 1f;
            player.ReceiveMonsterAttack(7, fakeMonsterPos);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            player.ReceiveMonsterAttack(3, player.transform.position);
            // 3 ������ �ֱ�
        }

        if (setting)
        {
            if (!isSettingPanelOn)
            {
                UIManager.Instance.OpenUI<SettingPanel>();
            }
            else
            {
                UIManager.Instance.CloseUI<SettingPanel>();
            }
        }
    }
}
