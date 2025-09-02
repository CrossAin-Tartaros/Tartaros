using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    bool isUsed;

    void Start()
    {
        //isUsed에 맞춰서 스프라이트 결정
    }

    //isUsed에 따라서 상호작용 가능 여부 결정


    //상호작용시 함수 호출
    void UseWater()
    {
        isUsed = true;
        //스프라이트 변경
        //저장
    }
}
