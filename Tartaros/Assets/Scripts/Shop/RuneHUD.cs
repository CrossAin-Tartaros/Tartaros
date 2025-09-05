using UnityEngine;

public class RuneHUD : UIBase
{
    [SerializeField] private GameObject attackIcon;      // ���� �� ������
    [SerializeField] private GameObject protectionIcon;  // ��� �� ������

    // UI�� ���� �� ����
    private void OnEnable()
    {
        // 1) ���� �� ���¸� ��ü ����
        RefreshAll();

        // 2) PlayerManager�� �̺�Ʈ ���� (�� ���°� �ٲ� �� �ڵ� �ݿ�)
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnRuneOwnedChanged += HandleRuneOwnedChanged;
    }

    // UI�� ���� �� ����
    private void OnDisable()
    {
        // �̺�Ʈ ���� ���� (�� �ϸ� �� �̵� �� �ߺ� ����/�޸� ���� �߻�)
        if (PlayerManager.Instance != null)
            PlayerManager.Instance.OnRuneOwnedChanged -= HandleRuneOwnedChanged;
    }

    // PlayerManager���� �� ���� ���� �̺�Ʈ�� �޾��� �� ȣ���
    private void HandleRuneOwnedChanged(RuneType type, bool owned)
    {
        SetOwned(type, owned);
    }

    // Ư�� �� �������� �Ѱų� ��
    public void SetOwned(RuneType type, bool owned)
    {
        if (type == RuneType.Attack && attackIcon)
            attackIcon.SetActive(owned);

        if (type == RuneType.Protection && protectionIcon)
            protectionIcon.SetActive(owned);
    }

    // ��ü �������� PlayerManager ���� �������� ����
    public void RefreshAll()
    {
        var pm = PlayerManager.Instance;
        if (pm == null) return;

        SetOwned(RuneType.Attack, pm.IsRuneOwnedIndex((int)RuneType.Attack));
        SetOwned(RuneType.Protection, pm.IsRuneOwnedIndex((int)RuneType.Protection));
    }
}
