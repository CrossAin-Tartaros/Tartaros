using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] GameObject shieldEffect;
    public bool IsShieldOn { get; private set; }
    [SerializeField] int initialShield;
    private int remainShield;


    public void GetShield()
    {
        shieldEffect.SetActive(true);
        IsShieldOn = true;  
        remainShield = initialShield;
        UIManager.Instance.GetUI<UIShield>().SetShield(remainShield);
    }

    public void UseShield()
    {
        remainShield--;
        UIManager.Instance.GetUI<UIShield>().SetShield(remainShield);

        if (remainShield == 0)
        {
            IsShieldOn = false;
            shieldEffect.SetActive(false);
            UIManager.Instance.GetUI<UIShield>().CloseUI();
        }
    }

}
