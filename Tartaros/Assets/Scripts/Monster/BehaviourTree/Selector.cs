using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    private List<Node> children;
    private int currentIndex = 0; // 마지막으로 실행한 자식 인덱스 기억

    public Selector(List<Node> children)
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
                case NodeState.Success:
                    // Selector는 하나라도 성공하면 전체 성공
                    currentIndex = 0;
                    state = NodeState.Success;
                    return state;

                case NodeState.Running:
                    // 실행 중이면 다음 틱에도 같은 자식부터 실행
                    state = NodeState.Running;
                    return state;

                case NodeState.Failure:
                    currentIndex++; // 다음 자식으로 진행
                    break;
            }
        }

        // 모든 자식이 실패했을 때
        currentIndex = 0;
        state = NodeState.Failure;
        return state;
    }
}
