using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    
    // TODO : 나중에 Entity로 관리해야함
    
    [field : Header("Stats")]
    [field: SerializeField] public MonsterType MonsterType { get; set; }
    [field: SerializeField] public int MaxHealth { get; set;} = 100;
    [field: SerializeField] public float MoveSpeed { get; set;} = 2f;
    public int CurrentHealth { get; set; }
    
    public MonsterAI AI{ get; set; }
    public MonsterAnimator Animator { get; set; }
    
    private void Awake()
    {
        CurrentHealth = MaxHealth;
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
