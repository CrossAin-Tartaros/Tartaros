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
        
        // 몬스터 인식 거리 내부이면 성공
        if (distance <= monsterAI.Recognize)
        {
            Debug.Log($"Can Recognize Player");
            state = NodeState.Success;
        }
            
        else 
            state = NodeState.Failure;
        return state;
    }
}
