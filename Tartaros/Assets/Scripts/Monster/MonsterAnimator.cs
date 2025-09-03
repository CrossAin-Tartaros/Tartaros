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

    private WaitForSecondsRealtime waitColorChange;
    private Coroutine damageCoroutine;

    public void Init(Monster monster)
    {
        this.monster = monster;
        waitColorChange = new WaitForSecondsRealtime(monster.data.StunWait);
    }
    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        data.Init();
    }

    public void DamageColored()
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }

        damageCoroutine = StartCoroutine(DamageColorChange());
    }

    IEnumerator DamageColorChange()
    {
        var block = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(block);
        block.SetColor("_Color", Color.red);
        spriteRenderer.SetPropertyBlock(block);
        StartAnimation(data.StunnedHash);
        yield return null;
        StopAnimation(data.StunnedHash);

        yield return waitColorChange;
        
        spriteRenderer.GetPropertyBlock(block);
        block.SetColor("_Color", Color.white);
        spriteRenderer.SetPropertyBlock(block);
        
        damageCoroutine = null;
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

    public void Stunned()
    {
        animator.SetTrigger(data.StunnedHash);
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

    public bool IsLeft()
    {
        return spriteRenderer.flipX;
    }
}
