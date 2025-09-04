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
    private Player player;

    public void Init(Player player)
    {
        this.player = player;
    }
    
    private void Start()
    {
        GetShield();
    }

    public void GetShield()
    {
        shieldEffect.SetActive(true);
        IsShieldOn = true;  
        remainShield = initialShield;
        UIManager.Instance.GetUI<UIShield>().SetShield(remainShield);
    }

    public void UseShield()
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
    }


}
