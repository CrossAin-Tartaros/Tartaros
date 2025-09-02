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
            //플레이어 teleportPosition 위치로 이동
            //플레이어 데미지
        }

        //몬스터가 함정에 걸리면 사망(Destroy or 체력 즉시 0)
    }
}
