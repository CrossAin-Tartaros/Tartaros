using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SceneBase
{
    public virtual void SceneLoading() { }
    public virtual void OnSceneEnter() { }
    public virtual void OnSceneExit() { }
}
