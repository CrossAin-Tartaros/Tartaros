using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : MonoBehaviour
{
    [SerializeField] private Button exitButton;

    private Shop owner;

    private void Awake()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnClickExit);
        }
        else
        {
            Debug.LogWarning("exitButton¿Ã ø¨∞· æ»µ ");
        }
    }

    public void SetOwner(Shop shop)
    {
        owner = shop;
    }

    private void OnClickExit()
    {
        if (owner != null)
        {
            owner.CloseShopUI();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
