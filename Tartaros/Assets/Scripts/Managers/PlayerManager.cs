using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private GameObject playerPrefab;
    public GameObject CurrentPlayerInstance { get; private set; }

    public Player Player {  get; private set; }
    public PlayerStat PlayerStat {  get; private set; }

    //저장될 데이터 모음
    int currentHealth = 10;
    int coin;
    int remainShield;
    public Dictionary<MapType, bool> waterUsed { get; private set; } = new Dictionary<MapType, bool>() { { MapType.Stage1, false }, { MapType.Boss, false } };

    private void Awake()
    {
        playerPrefab = Resources.Load<GameObject>("Player");
    }

    public void LoadPlayer(Vector2 position)
    {
        if (CurrentPlayerInstance != null)
            return;
        CurrentPlayerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        Player = CurrentPlayerInstance.gameObject.GetComponent<Player>();
        PlayerStat = CurrentPlayerInstance.gameObject.GetComponent<PlayerStat>();

        //플레이어에 저장된 정보 덮어쓰기
        PlayerStat.currentHP = currentHealth;
        UIManager.Instance.GetUI<HealthBar>().SetHealthBar(currentHealth);
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

    public void SavePlayer()
    {
        currentHealth = PlayerStat.currentHP;
    }

    public void GetCoin(int num)
    {
        coin += num;
        
        //UI에 반영
    }

    public void UseCoin(int num)
    {
        coin -= num;

        //UI에 반영
    }

 
    // TODO : Player 가지고 있게 해주시면 됩니다!
    // 세이브/로드도 여기서 하면 좋을 것 같아요
}
