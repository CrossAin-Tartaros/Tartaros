using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonsterAI : MonsterAI
{
    [field: SerializeField] public float BulletSpeed { get; set; } = 2f;
    [field: SerializeField] public Transform BulletSpawnPos { get; set; }
    [field: SerializeField] public GameObject BulletPrefab  { get; set; }

    [field: SerializeField] public Collider2D AttackCollider { get; set; }

    public override void Damaged()
    {
        BasePosition = transform.position;
        Monster.Animator.StopAllAnimations();
        Monster.Animator.StartAnimation(Monster.Animator.data.StunnedHash);
        Monster.Animator.DamageColored();
    }

    public override void BuildBT()
    {
        // Launcher 타입은 움직이지 않음
        Node isDeathNode = new IsDeathNode(this);

        Node checkStunNode = new CheckStunNode(this);
        Node stunWaitNode = new StunWaitNode(this, Monster.data.StunWait);
        Sequence stunSequence = new Sequence(new List<Node>
        {
            checkStunNode, stunWaitNode
        });
        
        Node canMeleeAttackPlayerNode = new CanAttackPlayerNode(this, Target, Monster.data.AttackRange);
        Node meleeAttackPlayerNode = new AttackPlayerNode(this, Target, MeleeAttack);
        Node meleeAttackWaitNode = new WaitNode(this, Monster.data.AttackWait);
        Sequence meleeAttackSequence = new Sequence(new List<Node>
        {
            canMeleeAttackPlayerNode, meleeAttackPlayerNode, meleeAttackWaitNode
        });
        
        
        Node canRangeAttackPlayerNode = new CanAttackPlayerNode(this, Target, Monster.data.Recognize);
        Node rangeAttackPlayerNode = new AttackPlayerNode(this, Target, RangeAttack);
        Node rangeAttackWaitNode = new WaitNode(this, Monster.data.AttackWait);
        Sequence rangeAttackSequence = new Sequence(new List<Node>
        {
            canRangeAttackPlayerNode, rangeAttackPlayerNode, rangeAttackWaitNode
        });
        
        
        Node canRecogPlayerNode = new CanRecogPlayerNode(this, Target);
        Sequence chaseSequence = new Sequence(new List<Node>
        {
            canRecogPlayerNode
        });
        
        Selector mainSelector = new Selector(new List<Node>
        {
            isDeathNode,
            stunSequence,
            meleeAttackSequence,
            rangeAttackSequence,
            chaseSequence,
        });
        
        monsterTree = new BehaviourTree(mainSelector);
    }

    public override void EndAttack()
    {
        Monster.Animator.StopAnimation(Monster.Animator.data.AttackHash);
    }

    public override void HandleAnimationEvent(string eventName)
    {
        Debug.Log($"{eventName}");
        Invoke(eventName, 0f);
    }
    
    public void RangeAttack()
    {
        Monster.Animator.StartAnimation(Monster.Animator.data.RangeHash);
        Monster.Animator.StartAnimation(Monster.Animator.data.AttackHash);
        Debug.Log("Range Attack");
    }
    
    public void MeleeAttack()
    {
        // Debug.Log("Warrior Attack Start");
        Monster.Animator.StartAnimation(Monster.Animator.data.AttackHash);
        Debug.Log("Melee Attack");
    }
    
    public void ParryingStart()
    {
        // Debug.Log("Parrying Start");
        AttackCollider.enabled = true;
    }

    public void ParryingStop()
    {
        // Debug.Log("Parrying Stop");
        AttackCollider.enabled = false;
    }

    public void StopAttack()
    {
        // Debug.Log("Attack Stop");
        Monster.Animator.StopAnimation(Monster.Animator.data.RangeHash);
        Monster.Animator.StopAnimation(Monster.Animator.data.AttackHash);
        isAttackDone = true;
    }
    
    public void ShootBullet()
    {
        Debug.Log("Launcher Shooting");
        Monster.Animator.StartAnimation(Monster.Animator.data.RangeHash);
        Monster.Animator.StartAnimation(Monster.Animator.data.AttackHash);
        isAttackDone = true;
        
        GameObject bullet = Instantiate(BulletPrefab, BulletSpawnPos.position, BulletSpawnPos.rotation);
        bullet.GetComponent<Bullet>().Init(Monster, BulletSpeed, Target.GetComponent<Player>().GetAimPoint());
    }
}
