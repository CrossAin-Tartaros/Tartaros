using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    private List<Node> children;

    public Selector(List<Node> children)
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
                // 실패하면 다음 자식으로 이동
                case NodeState.Failure:
                    continue;
                // 하나라도 성공하면 selector는 성공
                case NodeState.Success:
                    state = NodeState.Success;
                    return state;
                // 실행 중이면 그게 우선
                case NodeState.Running:
                    anyRunning = true;
                    break;
            }
        }
        
        state = anyRunning ? NodeState.Running : NodeState.Failure;
        return state;
    }
}
