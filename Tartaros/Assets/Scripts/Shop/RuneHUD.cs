using UnityEngine;

public class RuneHUD : UIBase
{
    [SerializeField] private GameObject attackIcon;      // AttackIcon
    [SerializeField] private GameObject protectionIcon;  // ProtectionIcon

    private void OnEnable() => RefreshAll();

    public void SetOwned(RuneType type, bool owned)
    {
        if (type == RuneType.Attack && attackIcon) attackIcon.SetActive(owned);
        if (type == RuneType.Protection && protectionIcon) protectionIcon.SetActive(owned);
    }

    public void RefreshAll()
    {
        var pm = PlayerManager.Instance;
        if (pm == null) return;
        SetOwned(RuneType.Attack, pm.IsRuneOwnedIndex((int)RuneType.Attack));
        SetOwned(RuneType.Protection, pm.IsRuneOwnedIndex((int)RuneType.Protection));
    }
}
