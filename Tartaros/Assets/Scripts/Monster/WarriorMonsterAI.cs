using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorMonsterAI : MonsterAI
{
    [field: SerializeField] public Collider2D AttackCollider { get; set; }
    
    public override void BuildBT()
    {
        Node isDeathNode = new IsDeathNode(this);
        
        Node checkStunNode = new CheckStunNode(this);
        Node stunWaitNode = new StunWaitNode(this, Monster.data.StunWait);
        Sequence stunSequence = new Sequence(new List<Node>
        {
            checkStunNode, stunWaitNode
        });
        
        
        Node canAttackPlayerNode = new CanAttackPlayerNode(this, Target, Monster.data.AttackRange);
        Node attackPlayerNode = new AttackPlayerNode(this, Target, MeleeAttack);
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
            stunSequence,
            attackSequence,
            chaseSequence,
            returnSequence,
            patrolSequence
        });
        
        monsterTree = new BehaviourTree(mainSelector);
    }
    public override void HandleAnimationEvent(string eventName)
    {
        // Debug.Log($"{eventName}");
        Invoke(eventName, 0f);
    }

    public void MeleeAttack()
    {
        // Debug.Log("Warrior Attack Start");
        Monster.Animator.StartAnimation(Monster.Animator.data.AttackHash);
    }

    public override void Damaged()
    {
        Monster.Animator.StopAllAnimations();
        Monster.Animator.StartAnimation(Monster.Animator.data.StunnedHash);
        Monster.Animator.DamageColored();
        Vector2 knockbackDir = (transform.position - Target.position).normalized * Monster.data.StunKnockBack;
        transform.position += (Vector3)knockbackDir;
    }

    public override void EndAttack()
    {
        StopAttack();
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
        Monster.Animator.StopAnimation(Monster.Animator.data.AttackHash);
        isAttackDone = true;
    }
}
