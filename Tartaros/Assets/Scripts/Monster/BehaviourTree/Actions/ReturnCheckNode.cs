using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnCheckNode : Node
{
    private MonsterAI monsterAI;

    public ReturnCheckNode(MonsterAI monsterAI)
    {
        this.monsterAI = monsterAI;
    }

    public override NodeState Evaluate()
    {
        if (monsterAI.isReturn)
        {
            state = NodeState.Success;
            monsterAI.Destination = monsterAI.BasePosition;
        }
        else
        {
            state = NodeState.Failure;
        }
        return state;
    }
}
