using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationData
{
    [SerializeField] private string moveString = "Move";
    [SerializeField] private string attackString = "Attack";
    [SerializeField] private string damagedString = "Damaged";
    [SerializeField] private string stunnedString = "Stunned";
    [SerializeField] private string deathString = "Death";
    [SerializeField] private string rangeString = "Range";
    
    public int MoveHash { get; private set; }
    public int AttackHash { get; private set; }
    public int DamagedHash { get; private set; }
    public int StunnedHash { get; private set; }
    
    public int DeathHash { get; private set; }
    public int RangeHash { get; private set; }

    public void Init()
    {
        MoveHash = Animator.StringToHash(moveString);
        AttackHash = Animator.StringToHash(attackString);
        DamagedHash = Animator.StringToHash(damagedString);
        StunnedHash = Animator.StringToHash(stunnedString);
        DeathHash = Animator.StringToHash(deathString);
        RangeHash = Animator.StringToHash(rangeString);
    }

    public List<int> GetDatas()
    {
        return new List<int>
            {
                MoveHash, AttackHash, DamagedHash, StunnedHash, DeathHash, RangeHash
            } ;
    }
}
