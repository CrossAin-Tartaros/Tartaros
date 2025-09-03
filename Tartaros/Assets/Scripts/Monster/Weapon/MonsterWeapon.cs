using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterWeapon : MonoBehaviour
{
    protected Monster monster;
    
    public virtual void Init(Monster monster)
    {
        this.monster = monster;
    }

    public abstract void Parry(int damage);
}
