using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public MonsterData data;
    public int CurrentHealth { get; set; }
    
    public MonsterAI AI{ get; set; }
    public MonsterAnimator Animator { get; set; }
    
    private void Awake()
    {
        if(data == null) Debug.LogError("Monster Data 연결 안됨");
        
        CurrentHealth = data.MaxHealth;
        AI = GetComponent<MonsterAI>();
        AI.Init(this);
        Animator = GetComponent<MonsterAnimator>();
        Animator.Init(this);
        
    }


    public void Die()
    {
        // TODO : 죽으면 진행
    }
    
}
