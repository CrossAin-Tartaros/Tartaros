using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    	public static T Instance
    	{
    		get
    		{
    			if(_instance == null)
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
}
