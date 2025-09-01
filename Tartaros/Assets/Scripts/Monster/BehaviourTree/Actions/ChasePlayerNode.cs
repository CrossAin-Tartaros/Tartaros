using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerNode : Node
{
    private Monster monster;
    private Transform player;
    private float distance;

    public ChasePlayerNode(Monster monster, Transform player)
    {
        this.monster = monster;
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        distance = Vector2.Distance(monster.transform.position, player.position);
        // 공격 가능 범위까지 왔으면 성공
        if (distance <= monster.AttackRange)
        {
            Debug.Log($"Can Attack Player");
            state = NodeState.Success;
            return state;
        }
        // 너무 멀어졌으면 실패
        else if (distance >= monster.Recognize)
        {
            Debug.Log($"Fail to chase player");
            state =  NodeState.Failure;
            return state;
        }
        // 성공/실패 아니면 진행 중
        else
        {
            monster.MoveToTarget(player.position);
            state = NodeState.Running;
            return state;
        }
    }
}
