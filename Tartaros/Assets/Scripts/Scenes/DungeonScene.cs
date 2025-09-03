using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScene : SceneBase
{
    public override void OnSceneEnter()
    {
        base.OnSceneEnter();

        UIManager.Instance.OpenUI<ScreenFader>();

        //���߿� ����� ���� ȣ���� �� �ְ� ���� ����
        MapManager.Instance.MoveToAnotherMap(MapType.Test.ToString(), true);
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
