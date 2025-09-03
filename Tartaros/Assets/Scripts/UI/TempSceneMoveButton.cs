using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneMoveButton : MonoBehaviour
{
    public void MoveScene()
    {
        UIManager.Instance.GetUI<ScreenFader>().FadeOut();
        
        SceneLoadManager.Instance.LoadScene(SceneType.DungeonScene);
    }
}
