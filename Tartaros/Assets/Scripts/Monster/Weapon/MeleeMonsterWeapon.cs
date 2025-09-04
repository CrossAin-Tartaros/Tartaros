using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeMonsterWeapon : MonsterWeapon
{
    [field: SerializeField] public float ParryDelay { get; set; } = 0.01f;
    public bool IsParried { get; set; }= false;
    

    public override void Parry(int damage)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        // Debug.Log("[Monster] Parried!");
        monster.Parried(damage);
        IsParried = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            IsParried = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log($"[Test] Trigger Exit : {other}");
        if (other.gameObject.CompareTag("Player") && other.gameObject.layer != LayerMask.NameToLayer("PlayerAttack"))
        {
            // Debug.Log($"[Test] Player이면서 무기가 아님.");
            if(other.gameObject.TryGetComponent(out Player player) && !IsParried)
            {
                // Debug.Log("[Test] Attack!");
                player.ReceiveMonsterAttack(monster.data.AttackDamage, monster.transform.position);
            }
        }
    }

    public override void EndParry()
    {
        IsParried = false;
    }
}
