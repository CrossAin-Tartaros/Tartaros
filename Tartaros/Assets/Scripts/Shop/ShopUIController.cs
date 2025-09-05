using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button buyAttackButton;       // 공격 룬 구매 버튼(좌클릭)
    [SerializeField] private Button buyProtectionButton;   // 방어 룬 구매 버튼(좌클릭)
    [SerializeField] private Button exitButton;            // 닫기 버튼

    [Header("Price Text (uGUI Text)")]
    [SerializeField] private Text attackPriceText;         // 공격 룬 가격 표시
    [SerializeField] private Text protectionPriceText;     // 방어 룬 가격 표시

    [Header("'보유중' 배지 오브젝트(선택)")]
    [SerializeField] private GameObject attackOwnedTag;    // 공격 룬 보유중 표시
    [SerializeField] private GameObject protectionOwnedTag;// 방어 룬 보유중 표시

    private Shop owner;

    private void Awake()
    {
        // 닫기
        if (exitButton != null) exitButton.onClick.AddListener(OnClickExit);
        else Debug.LogWarning("exitButton이 연결 안됨");

        // 구매 버튼 클릭 리스너: 인덱스로 전달 (Attack=0, Protection=1)
        if (buyAttackButton != null)
            buyAttackButton.onClick.AddListener(() => OnClickBuyIndex((int)RuneType.Attack));
        else Debug.LogWarning("buyAttackButton이 연결 안됨");

        if (buyProtectionButton != null)
            buyProtectionButton.onClick.AddListener(() => OnClickBuyIndex((int)RuneType.Protection));
        else Debug.LogWarning("buyProtectionButton이 연결 안됨");
    }

    private void OnEnable()
    {
        RefreshUI(); // UI 열릴 때 최신 상태 반영
    }

    public void SetOwner(Shop shop)
    {
        owner = shop;
        RefreshUI(); // 오너 세팅 시에도 갱신
    }

    //  핵심: 버튼 클릭 → PlayerManager에 "인덱스"로 구매 시도
    private void OnClickBuyIndex(int index)
    {
        var pm = PlayerManager.Instance;
        if (pm == null) return;

        // 이미 보유(=중복 구매 불가)
        if (pm.IsRuneOwnedIndex(index))
        {
            Debug.Log($"[ShopUI] 이미 보유 중: {(RuneType)index}");
            RefreshUI();
            return;
        }

        // 가격/코인 체크
        int price = pm.GetRunePriceForUI(index);
        if (!pm.HasCoins(price))
        {
            Debug.Log($"[ShopUI] 코인 부족. 필요:{price}, 보유:{pm.CurrentCoins}");
            return;
        }

        // 구매 > 자동 장착(능력치 즉시 반영 & bool[] true)
        bool ok = pm.TryBuyAndEquipRuneByIndex(index);
        if (ok)
        {
            var hud = FindObjectOfType<RuneHUD>();
            if (hud) hud.SetOwned((RuneType)index, true);  // 아이콘 활성화
        }
        Debug.Log($"[ShopUI] 구매 결과({(RuneType)index}): {ok}");

        RefreshUI();
    }

    private void RefreshUI()
    {
        var pm = PlayerManager.Instance;
        if (pm == null) return;

        // 가격 표시
        if (attackPriceText != null)
            attackPriceText.text = pm.GetRunePriceForUI((int)RuneType.Attack).ToString();
        if (protectionPriceText != null)
            protectionPriceText.text = pm.GetRunePriceForUI((int)RuneType.Protection).ToString();

        // 보유 여부
        bool hasAtk = pm.IsRuneOwnedIndex((int)RuneType.Attack);
        bool hasDef = pm.IsRuneOwnedIndex((int)RuneType.Protection);

        // 버튼 활성화: 미보유 && 코인 충분
        if (buyAttackButton != null)
            buyAttackButton.interactable = !hasAtk && pm.HasCoins(pm.GetRunePriceForUI((int)RuneType.Attack));
        if (buyProtectionButton != null)
            buyProtectionButton.interactable = !hasDef && pm.HasCoins(pm.GetRunePriceForUI((int)RuneType.Protection));

        // '보유중' 배지 표시
        if (attackOwnedTag != null) attackOwnedTag.SetActive(hasAtk);
        if (protectionOwnedTag != null) protectionOwnedTag.SetActive(hasDef);
    }

    private void OnClickExit()
    {
        if (owner != null) owner.CloseShopUI();
        else gameObject.SetActive(false);
    }
}
