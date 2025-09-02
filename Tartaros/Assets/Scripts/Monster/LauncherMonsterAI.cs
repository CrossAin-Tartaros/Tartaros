using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherMonsterAI : MonsterAI
{
    public override void BuildBT()
    {
        // Launcher 타입은 움직이지 않음
        Node isDeathNode = new IsDeathNode(this);

        Node canAttackPlayerNode = new CanAttackPlayerNode(this, Target, Monster.data.AttackRange);
        Node attackPlayerNode = new AttackPlayerNode(this, Target);
        Node attackWaitNode = new WaitNode(Monster.data.AttackWait);
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
            attackSequence,
            chaseSequence,
        });
        
        monsterTree = new BehaviourTree(mainSelector);
    }
    
    public override void HandleAnimationEvent(string eventName)
    {
        Debug.Log($"{eventName}");
        Invoke(eventName, 0f);
    }

    public override void Attack()
    {
        Monster.Animator.StartAnimation(Monster.Animator.data.MoveHash);
        Debug.Log("Launcher Attack");
    }

    public void ShootBullet()
    {
        Monster.Animator.StopAnimation(Monster.Animator.data.MoveHash);
    }
}
