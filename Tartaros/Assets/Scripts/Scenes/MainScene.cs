using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScene : SceneBase
{
    public override void OnSceneEnter()
    {
        base.OnSceneEnter();
        PlayerManager.Instance.SetPlayerPosition(new Vector2 (0, -3));
        UIManager.Instance.GetUI<ProgressUI>().SetProcress(PlayerManager.Instance.ProgressHighScore);
        if (SoundManager.Instance.musicClip != SoundManager.Instance.GetCurrentBGM())
        {
            SoundManager.Instance.ChangeBackGroundMusic(SoundManager.Instance.musicClip);
        }
    }

    public override void OnSceneExit()
    {
        base.OnSceneExit();
        PlayerManager.Instance.SavePlayer();
    }

    public override void SceneLoading()
    {
        base.SceneLoading();
    }
}
