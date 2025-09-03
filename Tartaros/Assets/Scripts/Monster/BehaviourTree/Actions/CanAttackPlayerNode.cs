using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanAttackPlayerNode : Node
{
    private MonsterAI monsterAI;
    private Transform player;
    private float maxAttackDistance;
    private float distance;
    
    public CanAttackPlayerNode(MonsterAI monsterAI,  Transform player, float maxAttackDistance)
    {
        this.monsterAI =  monsterAI;
        this.player = player;
        this.maxAttackDistance = maxAttackDistance;
    }
    public override NodeState Evaluate()
    {
        if (monsterAI.Monster.IsStunned)
        {
            state = NodeState.Success;
            return state;
        }
        distance = Vector2.Distance(monsterAI.transform.position, player.position);
        // Debug.Log($"{distance}");
        // 몬스터 공격 거리 내부이면 성공
        if (distance <= maxAttackDistance)
        {
            // Debug.Log($"Can Attack Player");
            if(monsterAI.Monster.data.MonsterType == MonsterType.Warrior)
                monsterAI.Monster.Animator.StopAnimation(monsterAI.Monster.Animator.data.MoveHash);
            monsterAI.Monster.Animator.ChangeHeadDirection(monsterAI.transform.position.x > player.position.x);
            state = NodeState.Success;
        }
        else
        {   
            monsterAI.EndAttack();
            state = NodeState.Failure;
        }
        return state;
    }
}
