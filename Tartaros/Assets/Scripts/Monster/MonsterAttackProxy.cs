using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackProxy : MonoBehaviour
{
    private Monster monster;
    

    public void Start()
    {
        monster = GetComponentInParent<Monster>();
    }

    public void OnAnimationEvent(string eventName)
    {
        GetComponentInParent<MonsterAI>()?.HandleAnimationEvent(eventName);
    }
}
