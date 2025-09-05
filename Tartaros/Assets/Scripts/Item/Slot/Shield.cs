using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] GameObject shieldEffect;
    [SerializeField] GameObject useShieldEffect;
    [SerializeField] private AudioClip brakeShieldSound;
    public bool IsShieldOn { get; private set; }
    [SerializeField] int initialShield;
    private int remainShield;
    public int RemainShield {get => remainShield;}
    private Player player;

    private float minTime;
    private float lastShieldUsedTime = 0f;

    public void Init(Player player)
    {
        this.player = player;
        minTime = player.InvincibleDuration;
    }

    private void Awake()
    {
        if (PlayerManager.Instance.Data != null)
        {
            GetOldShield(PlayerManager.Instance.Data.shieldCount);
        }
    }

    public void GetNewShield()
    {
        shieldEffect.SetActive(true);
        IsShieldOn = true;  
        remainShield = initialShield;
        UIManager.Instance.OpenUI<UIShield>();
        UIManager.Instance.GetUI<UIShield>().SetShield(remainShield);
        PlayerManager.Instance.RecordShield(remainShield);
    }

    public void GetOldShield(int remainShield)
    {
        if (remainShield <= 0)
            return;
        
        shieldEffect.SetActive(true);
        IsShieldOn = true;
        this.remainShield = remainShield;
        UIManager.Instance.OpenUI<UIShield>();
        UIManager.Instance.GetUI<UIShield>().SetShield(remainShield);
    }

    public void UseShield()
    {
        if (player.IsInvincible) return;
        if (Time.time - lastShieldUsedTime > minTime)
        {
            Debug.Log("Use Shield");
            remainShield--;
            UIManager.Instance.GetUI<UIShield>().SetShield(remainShield);
            GameObject go = Instantiate(useShieldEffect, player.GetAimPoint(0.8f),  Quaternion.identity);
            var psRenderer = go.GetComponent<ParticleSystemRenderer>();
            psRenderer.sortingOrder = 200;
            go.SetActive(true);
            SoundManager.Instance.PlayClip(brakeShieldSound, false);
        
            if (remainShield == 0)
            {
                IsShieldOn = false;
                shieldEffect.SetActive(false);
                UIManager.Instance.GetUI<UIShield>().CloseUI();
            }

            lastShieldUsedTime = Time.time;
            PlayerManager.Instance.RecordShield(remainShield);
        }
    }


}
