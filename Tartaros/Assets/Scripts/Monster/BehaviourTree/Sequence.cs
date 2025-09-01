using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    private List<Node> children;
    private int currentIndex = 0;

    // 자식 노드 받아오기
    public Sequence(List<Node> children)
    {
        this.children = children;
    }

    public override NodeState Evaluate()
    {
        while (currentIndex < children.Count)
        {
            NodeState result = children[currentIndex].Evaluate();

            switch (result)
            {
                // Sequence는 하나만 실패해도 전체 실패임
                case NodeState.Failure:
                    currentIndex = 0;
                    state = NodeState.Failure;
                    return state;

                // Running 인 경우에는 다음에 얘부터 실행하도록
                case NodeState.Running:
                    state = NodeState.Running;
                    return state;

                case NodeState.Success:
                    currentIndex++; // 다음 노드로 진행
                    break;
            }
        }
        
        currentIndex = 0;
        state = NodeState.Success;
        return state;
    }
}
