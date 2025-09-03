using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public MonsterData data;
    public int CurrentHealth { get; set; }
    public bool IsDead { get; set; } = false;
    public bool IsStunned { get; set; } = false;
    
    public MonsterAI AI{ get; set; }
    public MonsterAnimator Animator { get; set; }
    public MonsterWeapon Weapon { get; set; }
    
    private void Awake()
    {
        if(data == null) Debug.LogError("Monster Data 연결 안됨");
        
        CurrentHealth = data.MaxHealth;
        AI = GetComponent<MonsterAI>();
        AI.Init(this);
        Animator = GetComponent<MonsterAnimator>();
        Animator.Init(this);
        try
        {
            Weapon = GetComponentInChildren<MonsterWeapon>(true);
        }
        catch (Exception e)
        {
            Weapon = null;
        }
        Weapon?.Init(this);
    }


    public void Die()
    {
        Destroy(gameObject);
    }

    public void Damaged(int damage)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        Animator.Damaged();
        if (CurrentHealth <= 0)
        {
            IsDead = true;
        }
    }

    public void Parried(int damage)
    {
        Damaged(damage);
        IsStunned = true;
    }
    
}
