using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScene : SceneBase
{
    public override void OnSceneEnter()
    {
        base.OnSceneEnter();

        //���߿� ����� ���� ȣ���� �� �ְ� ���� ����
        MapManager.Instance.MoveToAnotherMap(MapType.Stage1, true);
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
