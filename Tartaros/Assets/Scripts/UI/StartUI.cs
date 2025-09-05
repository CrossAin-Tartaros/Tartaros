using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StartUI : UIBase
{
    [SerializeField] Button startButton;
    [SerializeField] Button exitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartButton);
        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    public void OnStartButton()
    {
        SceneLoadManager.Instance.LoadScene(SceneType.MainScene);
    }
    
}
