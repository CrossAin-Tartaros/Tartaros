using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : Node
{
    private MonsterAI monsterAI;
    private float startTime;
    private float maxMoveTime;

    public MoveNode(MonsterAI monsterAI, float maxMoveTime = 0f)
    {
        this.monsterAI =  monsterAI;
        this.maxMoveTime = maxMoveTime;
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
