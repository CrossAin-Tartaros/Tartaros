using UnityEngine;

public class Shop : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject shopUI;

    public bool isInteractable { get; set; } = true;

    public void OnInteract() //eŰ�� ������ PlayerInteract ȣ��
    {
        if (!isInteractable) return;

        if (shopUI == null)
        {
            Debug.LogWarning("[Shop] shopUI�� ����ȵ�");
            return;
        }

        OpenShopUI(); //��������
    }

    private void OpenShopUI()
    {
        shopUI.SetActive(true);

        isInteractable = false;

        Cursor.visible = true; Cursor.lockState = CursorLockMode.None;
        //Ŀ�� ���̰�

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
