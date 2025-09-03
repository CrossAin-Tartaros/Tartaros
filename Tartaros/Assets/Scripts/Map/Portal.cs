using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] bool isStartPosition;
    [Header("어디로 향하는 포탈?")]
    [SerializeField] MapType mapType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            MapManager.Instance.MoveToAnotherMap(mapType, !isStartPosition);
    }

}
