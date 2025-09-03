using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimator : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public MonsterAnimationData data = new();
    public Monster monster;


    public void Init(Monster monster)
    {
        this.monster = monster;
    }
    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        data.Init();
    }

    public bool HasParameter(int hash)
    {
        foreach (var param in animator.parameters)
        {
            if (param.nameHash == hash)
                return true;
        }
        return false;
    }

    public void StopAllAnimations()
    {
        foreach (var i in data.GetDatas())
        {
            if(HasParameter(i))
                animator.SetBool(i, false);
        }
    }

    public void Damaged()
    {
        animator.SetTrigger(data.DamagedHash);
    }
    

    public void StartAnimation(int param)
    {
        animator.SetBool(param, true);
    }

    public void StopAnimation(int param)
    {
        animator.SetBool(param, false);
    }

    public void ChangeHeadDirection(bool isLeft)
    {
        spriteRenderer.flipX = isLeft;
    }
}
