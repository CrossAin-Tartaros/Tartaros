using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animator Connection")]
    [SerializeField] private Animator anim;

    //Animator 파라미터와 이름 동일
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Crouch = Animator.StringToHash("Crouch");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Hit = Animator.StringToHash("Hit");

    private void Reset() //컴포넌트에 Animator 자동연결
    {
        if (!anim) anim = GetComponent<Animator>();
    }

    public void SetSpeed01(float Value) //0~1로 속도 값 설정
    {
        anim.SetFloat(Speed, Mathf.Clamp01(Value)); //실수값도 0~1로
    }

    public void SetCrouch(bool on) => anim.SetBool(Crouch, on); //엎드리기 여부

    //트리거 묶음
    public void TriggerJump() => anim.SetTrigger(Jump);
    public void TriggerAttack() => anim.SetTrigger(Attack);
    public void TriggerHit() => anim.SetTrigger(Hit);
}
