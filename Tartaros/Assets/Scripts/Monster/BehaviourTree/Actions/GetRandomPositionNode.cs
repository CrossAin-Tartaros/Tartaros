using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRandomPositionNode : Node
{
    private Monster monster;

    public GetRandomPositionNode(Monster monster)
    {
        this.monster = monster;
    }

    public override NodeState Evaluate()
    {
        if (!monster.isMoving)
        {
            monster.SetRandomDestination();    
            Debug.Log($"New Position : {monster.Destination.x}");
        }
        
        state = NodeState.Success;
        return state;
    }
}
