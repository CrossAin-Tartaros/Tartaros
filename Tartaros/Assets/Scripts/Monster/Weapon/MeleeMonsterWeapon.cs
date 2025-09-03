using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterWeapon : MonsterWeapon
{
    public override void Parry(int damage)
    {
        monster.Parried(damage);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어를 맞췄고, 그게 무기를 맞춘게 아닐 때
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
