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
        //�ݹ� �̺�Ʈ ����
        //�÷��̾� �ִ� ü�� �޾ƿ���
        //currentHealth = maxHealth;
    }

    private void OnDisable()
    {
        //���� ����
    }

    void SetHealthBar()
    {
        //currnetHealth = �÷��̾� ���� ü�� �޾ƿ���
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
