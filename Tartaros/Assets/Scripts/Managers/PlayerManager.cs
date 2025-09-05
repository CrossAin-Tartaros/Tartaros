using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Rune Settings")]
    [SerializeField] private int attackRunePrice = 10;
    [SerializeField] private int attackRuneBonus = 2;
    [SerializeField] private int protectionRunePrice = 10;
    [SerializeField] private int protectionRuneBonus = 2;

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

    private void Awake()
    {
        playerPrefab = Resources.Load<GameObject>("Player");
        InitRuneOwnedArray();
    }

    public void LoadPlayer(Vector2 position)
    {
        if (CurrentPlayerInstance != null)
            return;
        CurrentPlayerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        Player = CurrentPlayerInstance.gameObject.GetComponent<Player>();
        PlayerStat = CurrentPlayerInstance.gameObject.GetComponent<PlayerStat>();
        Shield = CurrentPlayerInstance.gameObject.GetComponent<Shield>();

        currentScore = 0;

        //플레이어에 저장된 정보 덮어쓰기

        currentHealth = PlayerStat.currentHP;
        UIManager.Instance.GetUI<HealthBar>().SetHealthBar(currentHealth);
        GetCoin(0);
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

        if (runeOwned[index] == owned)
            return false; // 값이 동일하면 아무 것도 하지 않음

        RuneType type = (RuneType)index;
        int bonus = GetRuneBonus(type);

        // owned=true => +bonus, owned=false => -bonus
        int delta = owned ? +bonus : -bonus;

        ApplyRuneDelta(type, delta);     // 능력치 증감
        runeOwned[index] = owned;        // 상태 저장
        LogStatBreakdown(type, bonus, owned); // 로그(출처 포함)

        return true;
    }
    // ==========================================
    // 내부 헬퍼들
    // ==========================================
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
            Debug.LogWarning("[Rune] PlayerStat 준비 전입니다(LoadPlayer 이전일 수 있음).");
            return;
        }

        switch (type)
        {
            case RuneType.Attack:
                PlayerStat.attack += delta;
                break;

            case RuneType.Protection:
                PlayerStat.defense += delta;
                break;
        }
    }

    // 로그: “무엇에 의해 스탯이 더해/빠졌는가?”
    private void LogStatBreakdown(RuneType type, int bonus, bool equipped)
    {
        // equipped=true  : after = 현재값, before = after - bonus, 표시 +bonus
        // equipped=false : after = 현재값, before = after + bonus, 표시 -bonus
        string tag = (type == RuneType.Attack) ? "AttackRune" : "ProtectionRune";

        if (type == RuneType.Attack)
        {
            int after = PlayerStat.attack;
            int before = equipped ? (after - bonus) : (after + bonus);
            string sign = equipped ? "+" : "-";
            Debug.Log($"[룬 {(equipped ? "장착" : "해제")}] 공격력 {before}({sign}{bonus}) = {after} [{tag}]");
        }
        else
        {
            int after = PlayerStat.defense;
            int before = equipped ? (after - bonus) : (after + bonus);
            string sign = equipped ? "+" : "-";
            Debug.Log($"[룬 {(equipped ? "장착" : "해제")}] 방어력 {before}({sign}{bonus}) = {after} [{tag}]");
        }
    }



    // TODO : Player 가지고 있게 해주시면 됩니다!
    // 세이브/로드도 여기서 하면 좋을 것 같아요
}
