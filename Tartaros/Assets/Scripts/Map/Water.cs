using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    public bool isInteractable { get; set; }
    
    public void OnInteract();

}


public class Water : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject used;
    [SerializeField] GameObject unused;

    public bool isInteractable { get; set; } = true;


    //상호작용
    public void OnInteract()
    {
        if (!isInteractable)
            return;

        UseWater();
    }
    void UseWater()
    {
        SetUsedWater();
        PlayerManager.Instance.waterUsed[MapManager.Instance.CurrentMapType] = true;
        PlayerManager.Instance.Player.ApplyHeal(5);
    }

    public void SetUsedWater()
    {
        isInteractable = false;
        unused.SetActive(false);
        used.SetActive(true);
    }

    
}
