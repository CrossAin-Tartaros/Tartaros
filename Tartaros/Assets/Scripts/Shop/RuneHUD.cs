using UnityEngine;

public class RuneHUD : UIBase
{
    [SerializeField] private GameObject attackIcon;      // 공격 룬 아이콘
    [SerializeField] private GameObject protectionIcon;  // 방어 룬 아이콘

    // UI가 켜질 때 실행
    private void OnEnable()
    {
        // 1) 현재 룬 상태를 전체 갱신
        RefreshAll();

        // 2) PlayerManager의 이벤트 구독 (룬 상태가 바뀔 때 자동 반영)
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnRuneOwnedChanged += HandleRuneOwnedChanged;
    }

    // UI가 꺼질 때 실행
    private void OnDisable()
    {
        // 이벤트 구독 해제 (안 하면 씬 이동 시 중복 구독/메모리 누수 발생)
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnRuneOwnedChanged -= HandleRuneOwnedChanged;
    }

    // PlayerManager에서 룬 상태 변경 이벤트를 받았을 때 호출됨
    private void HandleRuneOwnedChanged(RuneType type, bool owned)
    {
        SetOwned(type, owned);
    }

    // 특정 룬 아이콘을 켜거나 끔
    public void SetOwned(RuneType type, bool owned)
    {
        if (type == RuneType.Attack && attackIcon)
            attackIcon.SetActive(owned);

        if (type == RuneType.Protection && protectionIcon)
            protectionIcon.SetActive(owned);
    }

    // 전체 아이콘을 PlayerManager 상태 기준으로 갱신
    public void RefreshAll()
    {
        var pm = PlayerManager.Instance;
        if (pm == null) return;

        SetOwned(RuneType.Attack, pm.IsRuneOwnedIndex((int)RuneType.Attack));
        SetOwned(RuneType.Protection, pm.IsRuneOwnedIndex((int)RuneType.Protection));
    }
}
