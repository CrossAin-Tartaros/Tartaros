using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : SceneBase
{
    public override void OnSceneEnter()
    {
        base.OnSceneEnter();
        PlayerManager.Instance.SetPlayerPosition(new Vector2 (0, -3));
    }

    public override void OnSceneExit()
    {
        base.OnSceneExit();
    }

    public override void SceneLoading()
    {
        base.SceneLoading();
    }
}
