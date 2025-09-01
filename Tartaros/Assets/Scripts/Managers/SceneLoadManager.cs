using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public static SceneType CurrentScene {  get; private set; }
    public static SceneType PrevScene { get; private set; }

    private Dictionary<SceneType, SceneBase> _scenes = new Dictionary<SceneType, SceneBase>();

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 씬 정보를 딕셔너리에 등록
        _scenes.Add(SceneType.StartScene, new StartScene());
        _scenes.Add(SceneType.MainScene, new MainScene());
        _scenes.Add(SceneType.DungeonScene, new DungeonScene());
        _scenes.Add(SceneType.MapTestScene, new MapTestScene());

        // 시작 씬 설정
        //CurrentScene = SceneType.StartScene;
        CurrentScene = SceneType.MapTestScene;
    }



    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        // 굳이 OnSceneEnter를 콜백으로 처리하는 이유는, 씬이 완전히 로드되기 전에 실행되는 것을 방지하기 위함(gemini가 알려줌)
        _scenes[CurrentScene].OnSceneEnter();
    }

    public void LoadScene(SceneType scene)
    {
        // 이미 현재 씬이라면 아무것도 하지 않음
        if (CurrentScene == scene)
            return;

        // 현재 씬의 OnExit 로직을 먼저 실행
        _scenes[CurrentScene].OnSceneExit();
        // 씬 정보 업데이트
        PrevScene = CurrentScene;
        CurrentScene = scene;

        // **중요**: enum 이름을 실제 씬 파일 이름과 일치시켜야 합니다.
        SceneManager.LoadScene(scene.ToString());
    }
}
