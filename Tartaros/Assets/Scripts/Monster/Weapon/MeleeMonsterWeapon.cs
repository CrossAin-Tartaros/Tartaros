using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterWeapon : MonsterWeapon
{
    public override void Parry(int damage)
    {
        Debug.Log("[Monster] Parried!");
        monster.Parried(damage);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.layer != LayerMask.NameToLayer("PlayerAttack"))
        {
            if(other.gameObject.TryGetComponent(out Player player))
            {
                Debug.Log("Attack!");
                player.ReceiveMonsterAttack(monster.data.AttackDamage, monster.transform.position);
            }
        }
    }
}
