using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Rune Settings")]
    [SerializeField] private int attackRunePrice = 10;
    [SerializeField] private int attackRuneBonus = 2;
    [SerializeField] private int protectionRunePrice = 10;
    [SerializeField] private int protectionRuneBonus = 2;

    [Header("Starting Settings")]
    [SerializeField] private int startCoin = 10;
    [SerializeField] private int startHealth = 100;
    
    private int pendingAttackBonus = 0;
    private int pendingDefenseBonus = 0;

    private bool[] runeOwned;

    public int CurrentCoins => coin;
    public int GetRunePriceForUI(int index) => GetRunePrice((RuneType)index);
    public bool IsRuneOwnedIndex(int index) => IsValidRuneIndex(index) && runeOwned[index];

    private GameObject playerPrefab;
    public GameObject CurrentPlayerInstance { get; private set; }

    public Player Player {  get; private set; }
    public PlayerStat PlayerStat {  get; private set; }
    public Shield Shield { get; private set; }

    int currentScore;
    

    //저장될 데이터 모음
    int currentHealth = 10;
    int coin = 10; //시작코인 10
    public int ProgressHighScore { get; private set; }
    public Dictionary<MapType, bool> waterUsed { get; private set; } = new Dictionary<MapType, bool>() { { MapType.Stage1, false }, { MapType.Boss, false } };

    public PlayerData Data { get; set; }
    
    private void Awake()
    {
        playerPrefab = Resources.Load<GameObject>("Player");
        InitRuneOwnedArray();
    }

    private void Start()
    {
        LoadData();
        runeOwned = Data.runeOwned;
        SetCoin(Data.coin);
    }

    protected override void OnDestroy()
    {
        SaveData();
    }

    public void LoadPlayer(Vector2 position)
    {
        if (CurrentPlayerInstance != null) return;

        CurrentPlayerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        Player = CurrentPlayerInstance.GetComponent<Player>();
        PlayerStat = CurrentPlayerInstance.GetComponent<PlayerStat>();
        Shield = CurrentPlayerInstance.GetComponent<Shield>();

        currentScore = 0;

        // 체력 동기화
        PlayerStat.currentHP = currentHealth;
        UIManager.Instance.GetUI<HealthBar>().SetHealthBar(PlayerStat.currentHP);
        GetCoin(0);

        // 소유 중인 룬 보너스 계산
        int ownedAtk = IsRuneOwnedIndex((int)RuneType.Attack) ? attackRuneBonus : 0;
        int ownedDef = IsRuneOwnedIndex((int)RuneType.Protection) ? protectionRuneBonus : 0;

        // 최종 보너스 = 소유 보너스 + 보류 보너스
        int totalAtkBonus = ownedAtk + pendingAttackBonus;
        int totalDefBonus = ownedDef + pendingDefenseBonus;

        if (totalAtkBonus != 0) PlayerStat.attack += totalAtkBonus;
        if (totalDefBonus != 0) PlayerStat.defense += totalDefBonus;

        // 보류분 소진
        pendingAttackBonus = 0;
        pendingDefenseBonus = 0;
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

        ProgressHighScore = Mathf.Max(ProgressHighScore, currentScore);
    }

    public void GetCoin(int num)
    {
        coin += num;
        UIManager.Instance.GetUI<UICoin>().SetUICoin(coin);
    }

    public void UseCoin(int num)
    {
        coin -= num;

        UIManager.Instance.GetUI<UICoin>().SetUICoin(coin);
    }

    public void SetCoin(int num)
    {
        coin = num;
        UIManager.Instance.GetUI<UICoin>().SetUICoin(coin);
    }

    public void ProgressOne()
    {
        currentScore++;

        UIManager.Instance.GetUI<ProgressUI>().SetProcress(currentScore);
    }

    // RuneType의 개수에 맞춰 bool[]을 준비
    private void InitRuneOwnedArray()
    {
        int count = System.Enum.GetValues(typeof(RuneType)).Length;
        runeOwned = new bool[count]; // 전부 false로 초기화 (미보유)
    }

    public bool TryBuyAndEquipRuneByIndex(int index)
    {
        if (!IsValidRuneIndex(index))
        {
            Debug.LogWarning($"[Shop] 잘못된 룬 인덱스: {index}");
            return false;
        }

        if (IsRuneOwnedIndex(index))
        {
            Debug.Log($"[Shop] 이미 보유 중: {(RuneType)index}");
            return false;
        }

        RuneType type = (RuneType)index;
        int price = GetRunePrice(type);

        if (!HasCoins(price))
        {
            Debug.Log($"[Shop] 코인 부족. 필요:{price}, 보유:{coin}");
            return false;
        }

        // 코인 차감
        UseCoin(price);

        // 소유 상태를 true로 바꾸면서 '변경분'만큼 능력치 적용
        bool changed = SetRuneOwnedIndex(index, true);
        return changed;
    }

    private bool SetRuneOwnedIndex(int index, bool owned)
    {
        if (!IsValidRuneIndex(index)) return false;

        // 이미 true면 다시 적용 안 함
        if (runeOwned[index])
            return false;

        RuneType type = (RuneType)index;
        int bonus = GetRuneBonus(type);

        ApplyRuneDelta(type, bonus); // 무조건 +bonus
        runeOwned[index] = true;

        LogStatBreakdown(type, bonus, true);
        return true;
    }

    private bool IsValidRuneIndex(int index)
    {
        return runeOwned != null && index >= 0 && index < runeOwned.Length;
    }

    public bool HasCoins(int amount) => coin >= amount; // 유틸

    private int GetRunePrice(RuneType type)
    {
        switch (type)
        {
            case RuneType.Attack: return attackRunePrice;
            case RuneType.Protection: return protectionRunePrice;
            default: return 0;
        }
    }

    private int GetRuneBonus(RuneType type)
    {
        switch (type)
        {
            case RuneType.Attack: return attackRuneBonus;
            case RuneType.Protection: return protectionRuneBonus;
            default: return 0;
        }
    }

    // delta 만큼 스탯을 "증감" (+/- 둘 다 처리)
    private void ApplyRuneDelta(RuneType type, int delta)
    {
        if (PlayerStat == null)
        {
            Debug.LogWarning("[Rune] PlayerStat 준비 전, 보류 처리");
            if (type == RuneType.Attack) pendingAttackBonus += delta;
            else if (type == RuneType.Protection) pendingDefenseBonus += delta;
            return;
        }

        switch (type)
        {
            case RuneType.Attack: PlayerStat.attack += delta; break;
            case RuneType.Protection: PlayerStat.defense += delta; break;
        }
    }

    // 로그: “무엇에 의해 스탯이 더해/빠졌는가?”
    private void LogStatBreakdown(RuneType type, int bonus, bool equipped = true)
    {
        string tag = (type == RuneType.Attack) ? "AttackRune" : "ProtectionRune";

        if (type == RuneType.Attack)
        {
            int after = PlayerStat.attack;
            int before = after - bonus; // 장착 직전 값
            Debug.Log($"[룬 장착] 공격력 {before}(+{bonus}) = {after} [{tag}]");
        }
        else
        {
            int after = PlayerStat.defense;
            int before = after - bonus; // 장착 직전 값
            Debug.Log($"[룬 장착] 방어력 {before}(+{bonus}) = {after} [{tag}]");
        }
    }

    // TODO : Player 가지고 있게 해주시면 됩니다!
    // 세이브/로드도 여기서 하면 좋을 것 같아요

    public void SaveData()
    {
        Data.health = Player.stat.currentHP;
        Data.runeOwned = runeOwned;
        Data.coin = coin;
        Data.shieldCount = Shield.RemainShield;
        
        var str = JsonUtility.ToJson(Data, true);
        File.WriteAllText(Path.SavePath, str);
        Debug.Log("저장 완료" + Path.SavePath);
    }

    public void LoadData()
    {
        if (File.Exists(Path.SavePath))
        {
            string str = File.ReadAllText(Path.SavePath);
            PlayerData data =  JsonUtility.FromJson<PlayerData>(str);
            Debug.Log("로드 끝");
            Data = data;
        }
        else
        {
            Debug.Log("저장된 데이터가 없습니다. 새 캐릭터를 생성합니다.");
            NewData();
        }
    }

    public void NewData()
    {
        Data = new PlayerData
        {
            runeOwned = null,
            coin = startCoin,
            health = startHealth
        };
        
        Debug.Log("새 데이터 생성 완료");

        SaveData();
        LoadData();
    }
}
