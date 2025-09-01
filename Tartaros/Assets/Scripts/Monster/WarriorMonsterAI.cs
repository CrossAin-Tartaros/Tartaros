using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorMonsterAI : MonsterAI
{
    public override void BuildBT()
    {
        Node isDeathNode = new IsDeathNode(this);

        Node canRecogPlayerNode = new CanRecogPlayerNode(this, Target);
        Node chasePlayerNode = new ChasePlayerNode(this, Target);
        Sequence chaseSequence = new Sequence(new List<Node>
        {
            canRecogPlayerNode, chasePlayerNode
        });

        Node getRandomPosNode = new GetRandomPositionNode(this);
        Node moveNode = new MoveNode(this);
        Node patrolWaitNode = new WaitNode(this.PatrolWait);

        Sequence patrolSequence = new Sequence(new List<Node>
        {
            getRandomPosNode, moveNode, patrolWaitNode
        });

        Selector mainSelector = new Selector(new List<Node>
        {
            isDeathNode,
            chaseSequence,
            patrolSequence
        });
        
        monsterTree = new BehaviourTree(mainSelector);
    }
}
