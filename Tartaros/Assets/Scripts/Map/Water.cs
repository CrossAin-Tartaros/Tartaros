using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    bool isUsed;
    [SerializeField] GameObject used;
    [SerializeField] GameObject unused;

    //isUsed�� ���� ��ȣ�ۿ� ���� ���� ����


    //��ȣ�ۿ�� �Լ� ȣ��
    void UseWater()
    {
        SetUsedWater();
        //����
        //�÷��̾� ȸ��
    }

    public void SetUsedWater()
    {
        isUsed = true;
        unused.SetActive(false);
        used.SetActive(true);
    }

}
