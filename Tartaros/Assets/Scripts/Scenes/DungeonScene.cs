using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScene : SceneBase
{
    public override void OnSceneEnter()
    {
        base.OnSceneEnter();

        UIManager.Instance.OpenUI<ScreenFader>();

        //나중엔 저장된 맵을 호출할 수 있게 수정 예정
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
