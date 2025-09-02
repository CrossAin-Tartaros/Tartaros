using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerNode : Node
{
    private MonsterAI monsterAI;
    private Transform player;
    private float distance;
    
    public AttackPlayerNode(MonsterAI monsterAI, Transform player)
    {
        this.monsterAI = monsterAI;
        this.player = player;
    }
    
    public override NodeState Evaluate()
    {
        if (monsterAI.isAttackDone)
        {
            // 공격 끝난 경우
            monsterAI.isAttackDone = false;
            Debug.Log("Attack Done!");
            state = NodeState.Success;
        }
        // 이미 공격 중인 경우
        else if (monsterAI.Monster.Animator.animator.GetBool(monsterAI.Monster.Animator.data.AttackHash) 
            && !monsterAI.isAttackDone) 
        {
            state = NodeState.Running;    
        }
        // 공격 중이지 않은 경우, 공격 시도 
        else
        {
            monsterAI.Attack();
            monsterAI.isAttackDone = false;
            state = NodeState.Running;
        }
        return state;
    }
}
