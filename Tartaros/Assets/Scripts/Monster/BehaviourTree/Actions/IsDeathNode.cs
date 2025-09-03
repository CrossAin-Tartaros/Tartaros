using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsDeathNode : Node
{
    private MonsterAI monsterAI;
    private bool dieActionIsvoked = false;

    public IsDeathNode(MonsterAI monsterAI)
    {
        this.monsterAI = monsterAI;
    }

    public override NodeState Evaluate()
    {
        if (monsterAI.Monster.IsDead)
        {
            // 죽었을 때 액션 실행 안했으면 실행 하고 앞으로는 실행 안하도록 설정
            if (!dieActionIsvoked)
            {
                Debug.Log("Monster Die");
                monsterAI.Monster.Die();
                dieActionIsvoked = true;
            }
            state = NodeState.Success;
        } 
        else  state = NodeState.Failure; 
        return state;
    }
}
