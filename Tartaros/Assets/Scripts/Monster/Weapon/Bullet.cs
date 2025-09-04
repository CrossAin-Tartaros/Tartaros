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
    private Vector2 targetDirection;
    private SpriteRenderer spriteRenderer;
    
    public void Init(Monster monster, float speed, Vector2 targetPosition)
    {
        base.Init(monster);
        this.speed = speed;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        targetDirection = (targetPosition- (Vector2)transform.position).normalized * speed;
        if(targetDirection.x < 0) spriteRenderer.flipX = true;
    }

    public override void Parry(int damage)
    {
        parried = true;
        Debug.Log("Parry Bullet");
        parriedDirection = monster.AI.Target.GetComponent<Player>().IsLeft()
            ? -monster.AI.Target.right
            : monster.AI.Target.right;
        parriedDirection *= speed;
        if(parriedDirection.x < 0) spriteRenderer.flipX = true;
    }

    public override void EndParry()
    {
        Debug.Log("End Parry Bullet");
    }

    private void FixedUpdate()
    {
        if (!parried)
        {
            transform.position = (Vector2)transform.position + targetDirection * Time.deltaTime;
        }
        else
        {
            transform.position = (Vector2)transform.position + parriedDirection * Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
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
        // 몬스터 패링
        if (other.CompareTag("Monster") && parried)
        {
            if (other.TryGetComponent(out Monster monster))
            {
                monster.Damaged(monster.data.AttackDamage);
                Destroy(gameObject);
            }
        }
        // 벽에 부딛힘
        if (other.gameObject.layer == LayerMask.NameToLayer("OutOfMap") 
            || other.gameObject.layer == LayerMask.NameToLayer("Ground")
            || other.gameObject.layer == LayerMask.NameToLayer("LadderGround"))
        {
            Destroy(gameObject);
        }
    }
}
