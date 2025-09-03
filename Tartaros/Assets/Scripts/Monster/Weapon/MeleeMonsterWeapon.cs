using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterWeapon : MonsterWeapon
{
    public override void Parry(int damage)
    {
        monster.Parried(damage);
    }
}
