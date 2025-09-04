using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    public MonsterData data;

    [field: SerializeField] public float dropUpPosition { get; set; } = 0.5f;
    [field: SerializeField] public float dropRandomXPosition { get; set; } = 2f;
    public int CurrentHealth { get; set; }
    public bool IsDead { get; set; } = false;
    public bool IsStunned { get; set; } = false;
    
    public MonsterAI AI{ get; set; }
    public MonsterAnimator Animator { get; set; }
    public MonsterWeapon Weapon { get; set; }

    private Coroutine stunCoroutine;
    
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
        IsDead = true;
        PlayerManager.Instance.ProgressOne();
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        yield return new WaitForSecondsRealtime(2f);
        Vector2 dropPosition = 
            (Vector2)(transform.position 
                      + transform.up * dropUpPosition);
        if (data.DropItems.Length> 0)
        {
            for (int i = 0; i < data.DropItems.Length; i++)
            {
                // 위에서 스폰되는건 맞는데 좌우 랜덤값 줌
                GameObject go = Instantiate(data.DropItems[i],
                    dropPosition + (Vector2)transform.right * Random.Range(-dropRandomXPosition, dropRandomXPosition),
                    Quaternion.identity);
                go.SetActive(true);
            }
        }
        Destroy(gameObject);
    }

    public void Damaged(int damage)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (!IsStunned)
        {
            AI.Damaged();
        }
            
        if (CurrentHealth <= 0)
        {
            IsDead = true;
        }
    }

    public void Parried(int damage)
    {
        IsStunned = true;
        Damaged(damage);
        /*if(stunCoroutine == null)
            stunCoroutine = StartCoroutine(Stun()); */
    }

    public void OnColliderEnter2D(Collider2D other)
    {
        // 플레이어랑 부딛혔고, 무기가 아닐 때
        if (other.gameObject.CompareTag("Player") && other.gameObject.layer != LayerMask.NameToLayer("PlayerAttack"))
        {
            if(other.gameObject.TryGetComponent(out Player player))
            {
                player.ReceiveMonsterCollision(transform.position);
            }
        }
    }

    IEnumerator Stun()
    {
        Debug.Log("Behaviour Tree Paused.");
        Animator.StopAllAnimations();
        Animator.StartAnimation(Animator.data.StunnedHash);
        AI.isPausedBT = true;
        yield return new WaitForEndOfFrame();
        Animator.animator.speed = 0f;
        Animator.spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(data.StunWait);
        Animator.animator.speed = 1f;
        Animator.spriteRenderer.color = Color.white;
        AI.isPausedBT = false;
        IsStunned = false;
        Animator.StopAnimation(Animator.data.StunnedHash);
        stunCoroutine = null;
    }
}
