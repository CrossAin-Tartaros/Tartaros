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
    }

    private void OnDisable()
    {
        selectShield?.onClick.RemoveListener(OnSelectShield);
    }

    void OnSelectShield()
    {
        PlayerManager.Instance.Shield.GetShield();
        UIManager.Instance.CloseUI<ItemSelectPanel>();
    }
}
