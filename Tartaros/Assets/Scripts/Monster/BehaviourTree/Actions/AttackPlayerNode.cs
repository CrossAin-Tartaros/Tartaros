using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerNode : Node
{
    private MonsterAI monsterAI;
    private Transform player;
    private float distance;
    private Action attack;
    
    public AttackPlayerNode(MonsterAI monsterAI, Transform player, Action attack)
    {
        this.monsterAI = monsterAI;
        this.player = player;
        this.attack = attack;
    }
    
    public override NodeState Evaluate()
    {
        if (monsterAI.Monster.IsStunned)
        {
            state = NodeState.Success;
            return state;
        }
        // Debug.Log($"Attack Check - isAttackDone : {monsterAI.isAttackDone}");
        if (monsterAI.isAttackDone)
        {
            // 공격 끝난 경우
            monsterAI.isAttackDone = false;
            monsterAI.Monster.Animator.StopAnimation(monsterAI.Monster.Animator.data.AttackHash);
            // Debug.Log("Attack Done!");
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
            // Debug.Log("공격 시도");
            attack?.Invoke();
            monsterAI.isAttackDone = false;
            state = NodeState.Running;
        }
        return state;
    }
}
