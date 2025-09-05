using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelectPanel : UIBase
{
    [SerializeField] Button selectShield;

    private void OnEnable()
    {
        selectShield?.onClick.AddListener(OnSelectShield);
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
        selectShield?.onClick.RemoveListener(OnSelectShield);
    }

    void OnSelectShield()
    {
        PlayerManager.Instance.Shield.GetShield();
        UIManager.Instance.CloseUI<ItemSelectPanel>();
    }
}
