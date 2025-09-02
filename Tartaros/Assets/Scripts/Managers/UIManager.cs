using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public const string UIPrefabPath = "UI/";

    private bool _isCleaning;
    private Dictionary<string, UIBase> _uiDictionary = new Dictionary<string, UIBase>();

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }


    public void OpenUI<T>() where T : UIBase
    {
        var ui = GetUI<T>();
        ui?.OpenUI();
    }

    public void CloseUI<T>() where T : UIBase
    {
        if (IsExistUI<T>())
        {
            var ui = GetUI<T>();
            ui?.CloseUI();
        }
    }

    public T GetUI<T>() where T : UIBase
    {
        if (_isCleaning) return null;

        string uiName = GetUIName<T>();

        UIBase ui;
        if (IsExistUI<T>())
            ui = _uiDictionary[uiName];
        else
            ui = CreateUI<T>();

        return ui as T;
    }

    private T CreateUI<T>() where T : UIBase
    {
        if (_isCleaning) return null;

        string uiName = GetUIName<T>();
        if (_uiDictionary.TryGetValue(uiName, out var prevUi) && prevUi != null)
        {
            Destroy(prevUi.gameObject);
            _uiDictionary.Remove(uiName);
        }

        // 1. ������ �ε�
        string path = GetPath<T>();
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError($"[UIManager] Prefab not found: {path}");
            return null;
        }

        // 2. �ν��Ͻ� ����
        GameObject go = Instantiate(prefab);

        // 3. ������Ʈ ȹ��
        T ui = go.GetComponent<T>();
        if (ui == null)
        {
            Debug.LogError($"[UIManager] Prefab has no component : {uiName}");
            Destroy(go);
            return null;
        }

        // 4. Dictionary ���
        _uiDictionary[uiName] = ui;

        return ui;
    }

    public bool IsExistUI<T>() where T : UIBase
    {
        string uiName = GetUIName<T>();
        return _uiDictionary.TryGetValue(uiName, out var ui) && ui != null;
    }


    // ================================
    // path ����
    // ================================
    private string GetPath<T>() where T : UIBase
    {
        return UIPrefabPath + GetUIName<T>();
    }

    private string GetUIName<T>() where T : UIBase
    {
        return typeof(T).Name;
    }

    // ================================
    // ���ҽ� ����
    // ================================
    private void OnSceneUnloaded(Scene scene)
    {
        CleanAllUIs();
        StartCoroutine(CoUnloadUnusedAssets());
    }

    private void CleanAllUIs()
    {
        if (_isCleaning) return;
        _isCleaning = true;

        try
        {
            foreach (var ui in _uiDictionary.Values)
            {
                if (ui == null) continue;
                // Close ���μ��� �߰� ����
                Destroy(ui.gameObject);
            }
            _uiDictionary.Clear();
        }
        finally
        {
            _isCleaning = false;
        }
    }

    // UI �Ӹ� �ƴ϶� ��ü ������Ʈ ���� �ý������鿡���� ������ ����
    private IEnumerator CoUnloadUnusedAssets()
    {
        yield return Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
