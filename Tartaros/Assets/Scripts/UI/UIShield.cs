using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShield : UIBase
{
    [SerializeField] List<GameObject> shieldImages = new List<GameObject>();
    
    public void SetShield(int remainShield)
    {
        if (shieldImages.Count < remainShield)
        {
            Debug.LogError("[UIShield] UI�� �غ�� ���庸�� �� ���� ���尡 �ֽ��ϴ�");
            remainShield = shieldImages.Count;  
        }

        if (remainShield > 0)
        {
            for (int i = 0; i < remainShield; i++)
            {
                shieldImages[i].SetActive(true);
            }

            for (int i = 0; i < shieldImages.Count - remainShield; i++)
            {
                shieldImages[shieldImages.Count - 1 - i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < shieldImages.Count; i++)
            {
                shieldImages[i].SetActive(false);
            }
        }

    }
}
