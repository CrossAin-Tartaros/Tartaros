using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    private static bool isShuttingDown = false; // 종료 플래그

    public static T Instance
    {
    		get
    		{
            // 애플리케이션 종료 시 인스턴스 재생성 방지
            if (isShuttingDown)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' is already destroyed on application quit. Won't create again - returning null.");
                return null;
            }


            if (_instance == null)
    			{
    				_instance = FindObjectOfType<T>();
    				if(_instance == null)
    				{
    					GameObject go = new GameObject(typeof(T).ToString() + "(Singleton)");
    					_instance = go.AddComponent<T>();
    					if(!Application.isBatchMode)
    					{
    						if (Application.isPlaying)
    							DontDestroyOnLoad(go);
    					}
    				}
    			}
    			return _instance;
    		}
    }
    public static bool IsCreatedInstance()
    {
    		return (_instance != null);
    }

    // 종료 시 플래그 설정
    protected virtual void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    // 오브젝트 파괴 시에도 플래그 설정 (씬 전환 등)
    protected virtual void OnDestroy()
    {
        isShuttingDown = true;
    }
}
