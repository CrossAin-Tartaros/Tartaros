using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanRecogPlayerNode : Node
{
    private MonsterAI monsterAI;
    private Transform player;
    private float distance;

    public CanRecogPlayerNode(MonsterAI monsterAI,  Transform player)
    {
        this.monsterAI =  monsterAI;
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        distance = Vector2.Distance(monsterAI.transform.position, player.position);
        // Debug.Log($"{distance}");
        // 몬스터 공격 거리보다 멀고 인식 거리 내부이면 성공
        if (distance > monsterAI.Monster.data.AttackRange && distance <= monsterAI.Monster.data.Recognize)
        {
            // Debug.Log($"Can Recognize Player");
            monsterAI.Monster.Animator.ChangeHeadDirection(monsterAI.transform.position.x > player.position.x);
            state = NodeState.Success;
        }
            
        else 
            state = NodeState.Failure;
        return state;
    }
}
