using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;

    private static bool isShuttingDown = false; // ���� �÷���

    public static T Instance
    {
    		get
    		{
            // ���ø����̼� ���� �� �ν��Ͻ� ����� ����
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

    // ���� �� �÷��� ����
    protected virtual void OnApplicationQuit()
    {
        isShuttingDown = true;
    }

    // ������Ʈ �ı� �ÿ��� �÷��� ���� (�� ��ȯ ��)
    protected virtual void OnDestroy()
    {
        isShuttingDown = true;
    }
}
