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
        // 공격 가능 범위까지 왔으면 성공
        if (distance <= monsterAI.AttackRange)
        {
            monsterAI.Monster.Animator.StopAnimation(monsterAI.Monster.Animator.data.MoveHash);
            Debug.Log($"Can Attack Player");
            state = NodeState.Success;
            return state;
        }
        // 너무 멀어졌으면 실패
        else if (distance >= monsterAI.Recognize)
        {
            monsterAI.Monster.Animator.StopAnimation(monsterAI.Monster.Animator.data.MoveHash);
            Debug.Log($"Fail to chase player");
            state =  NodeState.Failure;
            return state;
        }
        // 성공/실패 아니면 진행 중
        else
        {
            monsterAI.Monster.Animator.ChangeHeadDirection(monsterAI.transform.position.x > player.position.x);
            monsterAI.Monster.Animator.StartAnimation(monsterAI.Monster.Animator.data.MoveHash);
            monsterAI.MoveToTarget(player.position);
            state = NodeState.Running;
            return state;
        }
    }
}
