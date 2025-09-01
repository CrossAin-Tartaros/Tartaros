using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRandomPositionNode : Node
{
    private MonsterAI monsterAI;

    public GetRandomPositionNode(MonsterAI monsterAI)
    {
        this.monsterAI = monsterAI;
    }

    public override NodeState Evaluate()
    {
        if (!monsterAI.isMoving)
        {
            monsterAI.SetRandomDestination();    
            Debug.Log($"New Position : {monsterAI.Destination.x}");
            monsterAI.Monster.Animator.ChangeHeadDirection(monsterAI.transform.position.x > monsterAI.Destination.x);
        }
        
        state = NodeState.Success;
        return state;
    }
}
