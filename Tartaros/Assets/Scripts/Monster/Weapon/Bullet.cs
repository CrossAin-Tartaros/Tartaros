using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bullet : MonsterWeapon
{

    public bool parried = false;
    private float speed;
    private Vector2 parriedDirection;
    private Vector2 targetPosition;
    
    public void Init(Monster monster, float speed, Vector2 targetPosition)
    {
        base.Init(monster);
        this.speed = speed;
        this.targetPosition = targetPosition;
    }

    public override void Parry(int damage)
    {
        parried = true;
        Debug.Log("Parry Bullet");
        parriedDirection = monster.AI.Target.GetComponent<Player>().IsLeft()
            ? -monster.AI.Target.right
            : monster.AI.Target.right;
        parriedDirection *= speed;
    }

    private void FixedUpdate()
    {
        if (!parried)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime );
        }
        else
        {
            transform.position = (Vector2)transform.position + parriedDirection * Time.deltaTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어 무기랑 부딛힘
        if (other.CompareTag("Player") && other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            Parry(monster.data.AttackDamage);
        }
        // 그냥 플레이어랑 부딛힘
        else if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Player player))
            {
                player.ReceiveMonsterAttack(monster.data.AttackDamage, transform.position);
                Destroy(gameObject);
            }
        }
        // 몬스터랑 부딛힘
        else if (other.CompareTag("Monster") && parried)
        {
            if (other.TryGetComponent(out Monster monster))
            {
                monster.Damaged(monster.data.AttackDamage);
                Destroy(gameObject);
            }
        }
        // 벽에 부딛힘
        else if (other.gameObject.layer == LayerMask.NameToLayer("OutOfMap"))
        {
            Destroy(gameObject);
        }
    }
}
