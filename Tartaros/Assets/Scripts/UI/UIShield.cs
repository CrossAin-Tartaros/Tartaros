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
            Debug.LogError("[UIShield] UI에 준비된 쉴드보다 더 많은 쉴드가 있습니다");
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
