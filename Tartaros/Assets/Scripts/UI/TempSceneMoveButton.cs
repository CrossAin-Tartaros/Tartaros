using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneMoveButton : MonoBehaviour
{
    public void MoveScene()
    {
        SceneLoadManager.Instance.LoadScene(SceneType.DungeonScene);
    }
}
