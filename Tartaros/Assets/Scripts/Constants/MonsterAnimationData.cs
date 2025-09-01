using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationData
{
    [SerializeField] private string moveString = "Move";
    [SerializeField] private string attackString = "Attack";
    [SerializeField] private string damagedString = "Damaged";
    
    public int MoveHash { get; private set; }
    public int AttackHash { get; private set; }
    public int DamagedHash { get; private set; }

    public void Init()
    {
        MoveHash = Animator.StringToHash(moveString);
        AttackHash = Animator.StringToHash(attackString);
        DamagedHash = Animator.StringToHash(damagedString);
    }
}
