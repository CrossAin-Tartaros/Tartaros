using UnityEngine;

//summary > �÷��̾��� ����(����&�̵�)�� ����/���

public class PlayerStat : MonoBehaviour
{
    [Header("Combat")]
    [Tooltip("�ִ� ü��")]
    public int maxHP = 10;

    [Tooltip("���� ü��")]
    public int currentHP = 10;

    [Tooltip("���ݷ�")]
    public int attack = 5;

    [Tooltip("���ݼӵ�")]
    public float attackSpeed = 1f; //1�ʿ� �Ѵ�

    [Tooltip("����")]
    public int defense = 3;

    [Header("Move")]
    [Tooltip("�ȴ� �ӵ�")]
    public float walkSpeed = 1f; //1ĭ�� 1��

    [Tooltip("�޸��� �ӵ�")]
    public float runSpeed = 2f;

    [Header("Jump")]
    [Tooltip("Ÿ�� ũ��")]
    public float tileSize = 1f;

    [Tooltip("���� ����")]
    public float jumpHeight = 2f; //2ĭ ����

    [Tooltip("���� �ð�")]
    public float jumpTime = 0.8f; //���� �ɸ��� �ð�

    public float jumpVelocity; //�����ӵ�
    public float gravityScale; //�߷�


    private void Awake()
    {
        RecalculateFromSpec();
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    private void OnValidate() => RecalculateFromSpec();
    
    void RecalculateFromSpec() //���� ��ġ ���
    {
        float h = jumpHeight * tileSize; //��ǥ ����
        float g = 2f * h / (jumpTime * jumpTime); //2 * ����ĭ * (�����ð� * �����ð�) > �߷� ���ӵ�

        jumpVelocity = g * jumpTime; //�ʱ� �ӵ�
        gravityScale = g / Mathf.Abs(Physics2D.gravity.y); //�߷�
    }

    public int ReduceDamage(int rawDamage) //���� ��� ���ذ���
    {
        int reduced = rawDamage - defense;
        return Mathf.Max(1, reduced); //�ּ� 1 �������� �ް�
    }
}
