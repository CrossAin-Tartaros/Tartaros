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
        //콜백 이벤트 구독
        //플레이어 최대 체력 받아오기
        //currentHealth = maxHealth;
    }

    private void OnDisable()
    {
        //구독 해지
    }

    void SetHealthBar()
    {
        //currnetHealth = 플레이어 현재 체력 받아오기
        if (currentHealth <= 0)
        {
            imageArray[currentImageIndex].SetActive(false);
        }

        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage >= 0.99)
            ChangeHealthBar(0);

        else if (healthPercentage > 0.66 && healthPercentage < 0.99)
            ChangeHealthBar(1);

        else if (healthPercentage > 0.66 && healthPercentage <= 0.83)
            ChangeHealthBar(2);

        else if (healthPercentage > 0.66 && healthPercentage <= 0.83)
            ChangeHealthBar(3);

        else if (healthPercentage > 0.66 && healthPercentage <= 0.83)
            ChangeHealthBar(4);

        else if (healthPercentage > 0.66 && healthPercentage <= 0.83)
            ChangeHealthBar(5);
    }

    void ChangeHealthBar(int index)
    {
        imageArray[currentImageIndex].SetActive(false);
        currentImageIndex = index;
        imageArray[currentImageIndex].SetActive(true);
    }
}
