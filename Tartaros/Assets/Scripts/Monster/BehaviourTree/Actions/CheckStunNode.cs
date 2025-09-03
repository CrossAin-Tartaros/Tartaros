using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckStunNode : Node
{
    private MonsterAI monsterAI;

    public CheckStunNode(MonsterAI monsterAI)
    {
        this.monsterAI = monsterAI;
    }

    public override NodeState Evaluate()
    {
        if (monsterAI.Monster.IsStunned)
        {
            monsterAI.Monster.Animator.animator.speed = 0;
            state = NodeState.Success;
        }
        else
        {
            state = NodeState.Failure;
        }

        return state;
    }
}
