using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public void ExitGame()
    {
        PlayerManager.Instance.SaveData();
        
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
