using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] Vector2 teleportPosition;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //�÷��̾� teleportPosition ��ġ�� �̵�
            //�÷��̾� ������
        }

        //���Ͱ� ������ �ɸ��� ���(Destroy or ü�� ��� 0)
    }
}
