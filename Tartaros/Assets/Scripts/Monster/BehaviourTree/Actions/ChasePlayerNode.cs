using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerNode : Node
{
    private MonsterAI monsterAI;
    private Transform player;
    private float distance;
    
    public ChasePlayerNode(MonsterAI monsterAI, Transform player)
    {
        this.monsterAI = monsterAI;
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        distance = Vector2.Distance(monsterAI.transform.position, player.position);
        if (distance <= monsterAI.Monster.data.AttackRange)
        {
            state = NodeState.Success;
        }
        else if (distance > monsterAI.Monster.data.Recognize)
        {
            state = NodeState.Failure;
        }
        else
        {
            monsterAI.Monster.Animator.StartAnimation(monsterAI.Monster.Animator.data.MoveHash);
            monsterAI.MoveToTarget(player.position);
            state = NodeState.Running;            
        }
        return state;
    }
}
