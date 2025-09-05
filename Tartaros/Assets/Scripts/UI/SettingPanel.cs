using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SettingPanel : UIBase
{
    [SerializeField] private Button exitButton;
    
    private void OnEnable()
    {
        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);
        // Debug.Log("게임 종료 리스너 추가");
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        exitButton.onClick.RemoveAllListeners();   
        Time.timeScale = 1f;
    }
}
