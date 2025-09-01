using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanRecogPlayerNode : Node
{
    private Monster monster;
    private Transform player;
    private float distance;

    public CanRecogPlayerNode(Monster monster,  Transform player)
    {
        this.monster =  monster;
        this.player = player;
    }

    public override NodeState Evaluate()
    {
        distance = Vector2.Distance(monster.transform.position, player.position);
        
        // 몬스터 인식 거리 내부이면 성공
        if (distance <= monster.Recognize)
        {
            Debug.Log($"Can Recognize Player");
            state = NodeState.Success;
        }
            
        else 
            state = NodeState.Failure;
        return state;
    }
}
