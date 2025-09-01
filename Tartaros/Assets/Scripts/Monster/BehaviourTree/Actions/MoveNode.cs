using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : Node
{
    private Monster monster;

    public MoveNode(Monster monster)
    {
        this.monster =  monster;
    }

    public override NodeState Evaluate()
    {
        if (monster.MoveToDestination())
        {
            Debug.Log("Move Done");
            state = NodeState.Success;
        }
        else
        {
            state = NodeState.Running;
        }

        return state;
    }
}
