using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : Node
{
    private float waitTime;
    private float startTime;

    public WaitNode(float waitTime)
    {
        this.waitTime = waitTime;
        startTime = 0;
    }

    public override NodeState Evaluate()
    {
        if (startTime == 0)
        {
            startTime = Time.time;
        }

        if (Time.time - startTime >= waitTime)
        {
            startTime = 0;
            state = NodeState.Success;
            return state;
        }
        
        state = NodeState.Running;
        return state;
    }
}
