using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] public AudioClip OnKiosk;
    
    public bool isInteractable { get; set; } = true;

    public void OnInteract() //e키를 누르면 PlayerInteract 호출
    {
        if (!isInteractable) return;

        if (shopUI == null)
        {
            Debug.LogWarning("[Shop] shopUI가 연결안됨");
            return;
        }

        SoundManager.Instance.PlayClip(OnKiosk, false);
        OpenShopUI(); //상점열기
    }

    private void OpenShopUI()
    {
        shopUI.SetActive(true);

        isInteractable = false;

        Cursor.visible = true; Cursor.lockState = CursorLockMode.None;
        //커서 보이게

        var uiController = shopUI.GetComponent<ShopUIController>();
        if (uiController != null)
        {
            uiController.SetOwner(this);
        }
    }

    public void CloseShopUI()
    {
        if (shopUI == null) return;

        shopUI.SetActive(false);
        isInteractable = true;
    }

}
