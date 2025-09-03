using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeMonsterWeapon : MonsterWeapon
{
    private bool isParried = false;
    
    public override void Parry(int damage)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        Debug.Log("[Monster] Parried!");
        monster.Parried(damage);
        isParried = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.layer != LayerMask.NameToLayer("PlayerAttack"))
        {
            if(other.gameObject.TryGetComponent(out Player player) && !isParried)
            {
                Debug.Log("Attack!");
                player.ReceiveMonsterAttack(monster.data.AttackDamage, monster.transform.position);
            }
        }
    }
}
