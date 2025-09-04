using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressUI : UIBase
{
    [SerializeField] TextMeshProUGUI progressText;
    [SerializeField] int allMonsterCount = 4;

    public void SetProcress(int currentProgres)
    {
        progressText.text = $"{currentProgres} / {allMonsterCount}";
    }
}
