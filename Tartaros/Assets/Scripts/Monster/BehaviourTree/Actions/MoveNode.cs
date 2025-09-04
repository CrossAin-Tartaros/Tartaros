using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : Node
{
    private MonsterAI monsterAI;
    private float startTime;
    private float maxMoveTime;
    private Vector3 lastPosition;
    private bool checkMove;
    private float moveThreshold;

    public MoveNode(MonsterAI monsterAI, bool checkMove = false, float maxMoveTime = 0f, float moveThreshold = 0.05f)
    {
        this.monsterAI =  monsterAI;
        this.checkMove = checkMove;
        this.maxMoveTime = maxMoveTime;
        this.moveThreshold = moveThreshold;
    }

    public override NodeState Evaluate()
    {
        if (state != NodeState.Running)
        {
            startTime = Time.time;
            lastPosition = monsterAI.transform.position;
        }
        
        if (monsterAI.MoveToDestination())
        {
            Debug.Log("Move Done");
            return EndMove();
        }
        
          
        monsterAI.Monster.Animator.StartAnimation(monsterAI.Monster.Animator.data.MoveHash);

        if (checkMove && Time.time - startTime > maxMoveTime)
        {
            if (Vector3.Distance(monsterAI.transform.position, lastPosition) < moveThreshold)
            {
                Debug.Log("No Move. Move Done");
                monsterAI.isStucked = true;
                monsterAI.stuckPosition = monsterAI.Monster.Animator.spriteRenderer.flipX ? -1 : 1;
                monsterAI.isMoving = false;
                return EndMove();
            }
            
            startTime = Time.time;
            lastPosition = monsterAI.transform.position;
        }
        
        state = NodeState.Running;
        return state;
    }

    NodeState EndMove()
    {
        monsterAI.Monster.Animator.StopAnimation(monsterAI.Monster.Animator.data.MoveHash);
        monsterAI.isReturn = false;
        state = NodeState.Success;
        return state;
    }
}
