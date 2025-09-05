using UnityEngine;

public class TempSceneMoveButton : MonoBehaviour
{
    public void MoveScene()
    {
        UIManager.Instance.GetUI<ScreenFader>().FadeOut();
        
        SceneLoadManager.Instance.LoadScene(SceneType.DungeonScene);
    }
}
