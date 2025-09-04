using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"{other.gameObject.name}과 부딛힘");
        if (other.gameObject.CompareTag("Player"))
        {
            // TODO : Player 코인 추가해주기
            Debug.Log("코인 먹음");
            Destroy(gameObject);
        }
    }
}
