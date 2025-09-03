using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorMonsterAI : MonsterAI
{
    
    [field: SerializeField] public Collider2D AttackCollider { get; set; }
    
    public override void BuildBT()
    {
        Node isDeathNode = new IsDeathNode(this);

        Node canAttackPlayerNode = new CanAttackPlayerNode(this, Target, Monster.data.AttackRange);
        Node attackPlayerNode = new AttackPlayerNode(this, Target, Attack);
        Node attackWaitNode = new WaitNode(this, Monster.data.AttackWait);
        Sequence attackSequence = new Sequence(new List<Node>
        {
            canAttackPlayerNode, attackPlayerNode, attackWaitNode
        });

        Node canRecogPlayerNode = new CanRecogPlayerNode(this, Target);
        Node checkBaseNode = new CheckBaseNode(this);
        Node chasePlayerNode = new ChasePlayerNode(this, Target);
        Sequence chaseSequence = new Sequence(new List<Node>
        {
            canRecogPlayerNode, checkBaseNode, chasePlayerNode
        });
        
        Node returnCheckNode = new ReturnCheckNode(this);
        Node returnMoveNode = new MoveNode(this);

        Sequence returnSequence = new Sequence(new List<Node>
        {
            returnCheckNode, returnMoveNode
        });

        Node getRandomPosNode = new GetRandomPositionNode(this);
        Node moveNode = new MoveNode(this);
        Node patrolWaitNode = new WaitNode(this, Monster.data.PatrolWait);

        Sequence patrolSequence = new Sequence(new List<Node>
        {
            getRandomPosNode, moveNode, patrolWaitNode
        });

        Selector mainSelector = new Selector(new List<Node>
        {
            isDeathNode,
            attackSequence,
            chaseSequence,
            returnSequence,
            patrolSequence
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
        // Debug.Log("Warrior Attack Start");
        Monster.Animator.StartAnimation(Monster.Animator.data.AttackHash);
    }

    public override void EndAttack()
    {
        StopAttack();
    }

    public void ParryingStart()
    {
        // Debug.Log("Parrying Start");
        AttackCollider.gameObject.SetActive(true);
    }

    public void ParryingStop()
    {
        // Debug.Log("Parrying Stop");
        AttackCollider.gameObject.SetActive(false);
    }

    public void StopAttack()
    {
        // Debug.Log("Attack Stop");
        Monster.Animator.StopAnimation(Monster.Animator.data.AttackHash);
        isAttackDone = true;
    }
}
