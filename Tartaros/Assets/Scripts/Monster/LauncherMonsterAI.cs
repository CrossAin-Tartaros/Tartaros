using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherMonsterAI : MonsterAI
{
    [field: SerializeField] public float BulletSpeed { get; set; } = 2f;
    [field: SerializeField] public Transform BulletSpawnPos { get; set; }
    [field: SerializeField] public GameObject BulletPrefab  { get; set; }



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
        
        Node canAttackPlayerNode = new CanAttackPlayerNode(this, Target, Monster.data.AttackRange);
        Node attackPlayerNode = new AttackPlayerNode(this, Target, RangeAttack);
        Node attackWaitNode = new WaitNode(this, Monster.data.AttackWait);
        Sequence attackSequence = new Sequence(new List<Node>
        {
            canAttackPlayerNode, attackPlayerNode, attackWaitNode
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
            attackSequence,
            chaseSequence,
        });
        
        monsterTree = new BehaviourTree(mainSelector);
    }
    public override void Damaged()
    {
        Monster.Animator.StopAllAnimations();
        Monster.Animator.StartAnimation(Monster.Animator.data.StunnedHash);
        Monster.Animator.DamageColored();
    }
    
    public override void HandleAnimationEvent(string eventName)
    {
        Debug.Log($"{eventName}");
        Invoke(eventName, 0f);
    }

    public void RangeAttack()
    {
        Monster.Animator.StartAnimation(Monster.Animator.data.AttackHash);
        Debug.Log("Launcher Attack");
    }

    public override void EndAttack()
    {
        Monster.Animator.StopAnimation(Monster.Animator.data.AttackHash);
    }

    public void ShootBullet()
    {
        Debug.Log("Launcher Shooting");
        Monster.Animator.StartAnimation(Monster.Animator.data.AttackHash);
        isAttackDone = true;


        // GameObject bullet = Instantiate(BulletPrefab, BulletSpawnPos.position, BulletSpawnPos.rotation);
        
        GameObject bullet = Instantiate(BulletPrefab, BulletSpawnPos);
        bullet.GetComponent<Bullet>().Init(Monster, BulletSpeed, Target.GetComponent<Player>().GetAimPoint());
        

        // Monster.Animator.StopAnimation(Monster.Animator.data.AttackHash);
    }

}
