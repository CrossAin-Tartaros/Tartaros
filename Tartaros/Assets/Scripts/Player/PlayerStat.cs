using UnityEngine;

//summary > 플레이어의 스탯(전투&이동)을 보관/계산

public class PlayerStat : MonoBehaviour
{
    [Header("Combat")]
    [Tooltip("최대 체력")]
    public int maxHP = 10;

    [Tooltip("현재 체력")]
    public int currentHP = 10;

    [Tooltip("공격력")]
    public int attack = 5;

    [Tooltip("공격속도")]
    public float attackSpeed = 1f; //1초에 한대

    [Tooltip("방어력")]
    public int defense = 3;

    [Header("Move")]
    [Tooltip("걷는 속도")]
    public float walkSpeed = 1f; //1칸에 1초

    [Tooltip("달리는 속도")]
    public float runSpeed = 2f;

    [Header("Jump")]
    [Tooltip("타일 크기")]
    public float tileSize = 1f;

    [Tooltip("점프 높이")]
    public float jumpHeight = 2f; //2칸 점프

    [Tooltip("점프 시간")]
    public float jumpTime = 0.8f; //점프 걸리는 시간

    public float jumpVelocity; //점프속도
    public float gravityScale; //중력


    private void Awake()
    {
        RecalculateFromSpec();
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    private void OnValidate() => RecalculateFromSpec();
    
    void RecalculateFromSpec() //점프 수치 계산
    {
        float h = jumpHeight * tileSize; //목표 높이
        float g = 2f * h / (jumpTime * jumpTime); //2 * 점프칸 * (점프시간 * 점프시간) > 중력 가속도

        jumpVelocity = g * jumpTime; //초기 속도
        gravityScale = g / Mathf.Abs(Physics2D.gravity.y); //중력
    }

    public int ReduceDamage(int rawDamage) //방어력 기반 피해감소
    {
        int reduced = rawDamage - defense;
        return Mathf.Max(1, reduced); //최소 1 데미지는 받게
    }
}
