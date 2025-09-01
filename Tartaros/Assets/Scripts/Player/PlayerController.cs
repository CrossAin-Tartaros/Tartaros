using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Player player; // 이동/점프 등 실제 로직
    [SerializeField] private PlayerAnimation anim; // 애니메이터 전담

    [Header("Animation Blend")]
    [SerializeField] private float speedLerp = 8f;

    private float curSpeed; //실제 Animator에 들어갈 속도

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
        float x = Input.GetAxisRaw("Horizontal"); // 좌우 이동
        bool run = Input.GetKey(KeyCode.LeftShift); // 달리기
        bool jump = Input.GetKeyDown(KeyCode.Space); // 점프
        bool attack = Input.GetMouseButtonDown(0); // 공격
        bool crouchHeld = Input.GetKey(KeyCode.S); //엎드리기

        player.Move(x, run); //player.cs에서 호출

        float moveInput01 = Mathf.Clamp01(Mathf.Abs(x));
        float targetSpeed01 = moveInput01 * (run ? 1f : 0.5f);
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed01, Time.deltaTime * speedLerp);
        if (targetSpeed01 == 0f && curSpeed < 0.05f) curSpeed = 0f;
        curSpeed = Mathf.Clamp01(curSpeed);
        anim.SetSpeed01(curSpeed);

        if (targetSpeed01 == 0f && curSpeed < 0.05f)
            curSpeed = 0f;

        anim.SetCrouch(crouchHeld);

        if (jump)
        {
            player.Jump();
            anim.TriggerJump();
        }

        if (attack)
        {
            anim.TriggerAttack();
        }

        // 데미지 테스트
        if (Input.GetKeyDown(KeyCode.H))
        {
            anim.TriggerHit();
        }
    }
}
