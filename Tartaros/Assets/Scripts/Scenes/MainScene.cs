using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MainScene : SceneBase
{
    public override async void OnSceneEnter()
    {
        MapManager.Instance.MoveToAnotherMap(MapType.Town, true);
        try
        {
            base.OnSceneEnter();
            if (UIManager.Instance == null)
            {
                await WaitForUIManager();    
            }
            UIManager.Instance.GetUI<ProgressUI>().SetProcress(PlayerManager.Instance.ProgressHighScore);
            PlayerManager.Instance.SetPlayerPosition(new Vector2 (0, -3));
            if (SoundManager.Instance.musicClip != SoundManager.Instance.GetCurrentBGM())
            {
                SoundManager.Instance.ChangeBackGroundMusic(SoundManager.Instance.musicClip);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    public override void OnSceneExit()
    {
        base.OnSceneExit();
        PlayerManager.Instance.SavePlayer();
    }

    async Task WaitForUIManager()
    {
        while (UIManager.Instance == null)
            await Task.Yield();
        Debug.Log("Now UIManager is not null");
    }

    public override void SceneLoading()
    {
        base.SceneLoading();
    }
}
