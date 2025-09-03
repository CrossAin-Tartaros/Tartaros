using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : Node
{
    private MonsterAI monsterAI;

    public MoveNode(MonsterAI monsterAI)
    {
        this.monsterAI =  monsterAI;
    }

    public override NodeState Evaluate()
    {
        if (monsterAI.MoveToDestination())
        {
            Debug.Log("Move Done");
            monsterAI.Monster.Animator.StopAnimation(monsterAI.Monster.Animator.data.MoveHash);
            monsterAI.isReturn = false;
            state = NodeState.Success;
        }
        else
        {  
            monsterAI.Monster.Animator.StartAnimation(monsterAI.Monster.Animator.data.MoveHash);
            state = NodeState.Running;
        }

        return state;
    }
}
