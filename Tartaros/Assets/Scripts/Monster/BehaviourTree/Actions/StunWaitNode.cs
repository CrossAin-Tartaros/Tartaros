using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunWaitNode : Node
{
    private MonsterAI monsterAI;
    private float waitTime;
    private float startTime;
    private bool isStarted = false;

    public StunWaitNode(MonsterAI monsterAI, float waitTime)
    {
        this.waitTime = waitTime;
        this.monsterAI = monsterAI;
        startTime = 0;
    }

    public override NodeState Evaluate()
    {
        if (!isStarted)
        {
            startTime = Time.time;
            monsterAI.Damaged();
            monsterAI.Monster.Animator.animator.speed = 0f;
            isStarted = true;
        }

        if (Time.time - startTime >= waitTime)
        {
            isStarted = false;
            startTime = 0;
            state = NodeState.Success;
            monsterAI.Monster.Animator.animator.speed = 1f;
            monsterAI.Monster.IsStunned = false;
            monsterAI.Monster.Animator.StopAnimation(monsterAI.Monster.Animator.data.StunnedHash);
            monsterAI.Monster.Weapon.EndParry();
            return state;
        }
        
        state = NodeState.Running;
        return state;
    }
}
