using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase
{
    public virtual void SceneLoading() { }

    public virtual void OnSceneEnter()
    {
        if(PlayerManager.Instance.Player != null)
            PlayerManager.Instance.Player.SceneChanging = false;
    }

    public virtual void OnSceneExit()
    {
        if(PlayerManager.Instance.Player != null)
            PlayerManager.Instance.Player.SceneChanging = true;
        SoundManager.Instance.ClearClips();
        
    }
}
