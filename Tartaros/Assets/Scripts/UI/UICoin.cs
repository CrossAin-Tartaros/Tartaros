using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICoin : UIBase
{
    [SerializeField] TextMeshProUGUI coinText;

    public void SetUICoin(int coinNum)
    {
        coinText.text = coinNum.ToString();
    }
}
