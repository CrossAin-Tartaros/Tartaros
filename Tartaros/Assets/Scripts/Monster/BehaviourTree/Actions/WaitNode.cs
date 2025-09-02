using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitNode : Node
{
    private MonsterAI monsterAI;
    private float waitTime;
    private float startTime;

    public WaitNode(MonsterAI monsterAI, float waitTime)
    {
        this.waitTime = waitTime;
        this.monsterAI = monsterAI;
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
            monsterAI.Monster.Animator.StopAllAnimations();
            return state;
        }
        
        state = NodeState.Running;
        return state;
    }
}
