using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBaseNode : Node
{
    private MonsterAI monsterAI;
    private float distance;

    public CheckBaseNode(MonsterAI monsterAI)
    {
        this.monsterAI =  monsterAI;
    }

    public override NodeState Evaluate()
    {
        distance = Vector2.Distance(monsterAI.transform.position, monsterAI.BasePosition);
        Debug.Log($"{distance}");
        // 베이스 위치랑 그리 멀지 않으면 성공
        if (distance <= monsterAI.Monster.data.MaxBaseDistance)
        {
            state = NodeState.Success;
        }
        // 베이스 위치랑 너무 멀면 실패
        else
        {
            monsterAI.Monster.Animator.ChangeHeadDirection(monsterAI.transform.position.x > monsterAI.BasePosition.x);
            monsterAI.isReturn = true;
            state = NodeState.Failure;
        }
            
        return state;
    }
}
