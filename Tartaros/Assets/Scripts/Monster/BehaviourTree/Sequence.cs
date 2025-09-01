using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    private List<Node> children;

    // 자식 노드 받아오기
    public Sequence(List<Node> children)
    {
        this.children = children;
    }

    public override NodeState Evaluate()
    {
        bool anyRunning = false; // 실행 중인 노드가 있는지 체크하는 부분


        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                // 하나라도 실패하면 안됨.
                case NodeState.Failure:
                    state = NodeState.Failure;
                    return state;
                // 성공하면 다음 자식으로 이동
                case NodeState.Success:
                    continue;
                // 어떤 자식 노드가 실행 중이라면 이 Sequence 노드의 상태도 실행 중임.
                case NodeState.Running:
                    anyRunning = true;
                    break;
            }
        }
        
        state = anyRunning ? NodeState.Running : NodeState.Success;
        return state;
    }
}
