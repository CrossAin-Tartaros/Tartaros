using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animator Connection")]
    [SerializeField] private Animator anim;

    //Animator �Ķ���Ϳ� �̸� ����
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hit = Animator.StringToHash("Hit");

    private void Reset() //������Ʈ�� Animator �ڵ�����
    {
        if (!anim) anim = GetComponent<Animator>();
    }

    public void SetSpeed01(float Value) //0~1�� �ӵ� �� ����
    {
        anim.SetFloat(Speed, Mathf.Clamp01(Value)); //�Ǽ����� 0~1��
    }

    public void SetCrouch(bool on) => anim.SetBool(Crouch, on); //���帮�� ����

    //Ʈ���� ����
    public void TriggerJump() => anim.SetTrigger(Jump);
    public void TriggerAttack() => anim.SetTrigger(Attack);
    public void TriggerHit() => anim.SetTrigger(Hit);
}
