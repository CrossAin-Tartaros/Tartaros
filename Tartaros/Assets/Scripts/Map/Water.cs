using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    bool isUsed;
    [SerializeField] GameObject used;
    [SerializeField] GameObject unused;

    //isUsed에 따라서 상호작용 가능 여부 결정


    //상호작용시 함수 호출
    void UseWater()
    {
        SetUsedWater();
        //저장
        //플레이어 회복
    }

    public void SetUsedWater()
    {
        isUsed = true;
        unused.SetActive(false);
        used.SetActive(true);
    }

}
