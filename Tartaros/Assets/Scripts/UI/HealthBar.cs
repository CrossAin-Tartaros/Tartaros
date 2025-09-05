using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : UIBase
{
    [SerializeField] GameObject[] imageArray = new GameObject[6];

    private float maxHealth;
    private float currentHealth;
    private int currentImageIndex = 0;

    private void OnEnable()
    {
        if(PlayerManager.Instance.Player == null)
            PlayerManager.Instance.LoadPlayer(new Vector2(0, -100));    
        PlayerManager.Instance.Player.onPlayerHealthChange += SetHealthBar;
        maxHealth = PlayerManager.Instance.PlayerStat.maxHP;
        currentHealth = maxHealth;
    }

    private void OnDisable()
    {
        if (PlayerManager.Instance.Player != null)
            PlayerManager.Instance.Player.onPlayerHealthChange -= SetHealthBar;
    }

    public void SetHealthBar(float newHealth)
    {
        Debug.LogWarning(newHealth);
        
        //플레이어 현재 체력 받아오기
        currentHealth = newHealth;

        if (currentHealth <= 0)
        {
            imageArray[currentImageIndex].SetActive(false);
        }

        float healthPercentage = currentHealth / maxHealth;

        Debug.LogWarning(healthPercentage);

        if (healthPercentage >= 0.99)
            ChangeHealthBar(0);

        else if (healthPercentage > 0.8 && healthPercentage < 0.99)
            ChangeHealthBar(1);

        else if (healthPercentage > 0.6 && healthPercentage <= 0.8)
            ChangeHealthBar(2);

        else if (healthPercentage > 0.4 && healthPercentage <= 0.6)
            ChangeHealthBar(3);

        else if (healthPercentage > 0.2 && healthPercentage <= 0.4)
            ChangeHealthBar(4);

        else if (healthPercentage > 0 && healthPercentage <= 0.2)
            ChangeHealthBar(5);
    }

    void ChangeHealthBar(int index)
    {
        imageArray[currentImageIndex].SetActive(false);
        currentImageIndex = index;
        imageArray[currentImageIndex].SetActive(true);
    }
}
