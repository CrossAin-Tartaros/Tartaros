using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonScene : SceneBase
{
    public override void OnSceneEnter()
    {
        base.OnSceneEnter();

        //나중엔 저장된 맵을 호출할 수 있게 수정 예정
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
