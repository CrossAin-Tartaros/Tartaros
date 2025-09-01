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
