using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private GameObject playerPrefab;
    public GameObject CurrentPlayerInstance { get; private set; }

    public Player Player {  get; private set; }

    private void Awake()
    {
        playerPrefab = Resources.Load<GameObject>("Player");
    }

    public void LoadPlayer(Vector2 position)
    {
        CurrentPlayerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        
        //플레이어에 저장된 스탯 덮어쓰기
    }

    public void SetPlayerPosition(Vector2 position)
    {
        if (CurrentPlayerInstance == null)
        {
            LoadPlayer(position);
        }
        else
        {
            Rigidbody2D currentRb;
            currentRb = CurrentPlayerInstance.GetComponent<Rigidbody2D>();
            currentRb.MovePosition(position);
        }
    }


    //저장될 데이터 모음
    float currentHealth;
    public Dictionary<MapType, bool> waterUsed { get; private set; } = new Dictionary<MapType, bool>() { { MapType.Stage1, false }, { MapType.Boss, false } };


    
    // TODO : Player 가지고 있게 해주시면 됩니다!
    // 세이브/로드도 여기서 하면 좋을 것 같아요
}
