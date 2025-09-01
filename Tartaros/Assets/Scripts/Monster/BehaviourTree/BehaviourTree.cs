using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree
{
    private Node root;

    public BehaviourTree(Node root)
    {
        this.root = root;
    }

    public void Tick()
    {
        root.Evaluate();
    }
}
