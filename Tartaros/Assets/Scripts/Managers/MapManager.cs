using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private Dictionary<MapType, MapData> mapDatas = new();
    private MapData currentMapData;
    private GameObject mapInstance;

    private ScreenFader screenFader;

    private GameObject waterPrefab;
    private Water currentWater;

    private List<GameObject> currentMonsterList = new();

    //Enum/MapType에서 Resources 폴더 내 Mapdata 이름을 저장해서 사용.
    private void Awake()
    {
        mapDatas.Add(MapType.Test, Resources.Load<MapData>("Maps/" + MapType.Test.ToString()));
        mapDatas.Add(MapType.Stage1, Resources.Load<MapData>("Maps/" + MapType.Stage1.ToString()));
        mapDatas.Add(MapType.Boss, Resources.Load<MapData>("Maps/" + MapType.Boss.ToString()));

        waterPrefab = Resources.Load<GameObject>("Maps/" + "Fontaine");
    }


    void LoadNewMap(MapType mapType)
    {
        //초기화
        ClearMap();

        currentMapData = mapDatas[mapType];

        //기본 구조물 로드
        mapInstance = Instantiate(currentMapData.mapPrefab);

        //샘물 로드
        if (currentMapData.waterPosition != Vector2.zero)
        {
            currentWater = Instantiate(waterPrefab, currentMapData.waterPosition, Quaternion.identity).gameObject.GetComponent<Water>();

            //저장 데이터에 따라서 샘물 사용여부 결정
            if (PlayerManager.Instance.waterUsed[mapType])
                currentWater.SetUsedWater();
        }


        if (mapInstance != null)
            Debug.Log($"{mapType} 로드");
    }

    void LoadNewEntity(bool isStartPosition)
    {
        //몬스터 소환
        for (int i = 0; i < currentMapData.monsterSpawnList.Count; i++)
        {

            GameObject obj = Instantiate(currentMapData.monsterSpawnList[i].monsterPrefab, currentMapData.monsterSpawnList[i].position, Quaternion.identity);
            currentMonsterList.Add(obj);
        }

        //플레이어 위치 설정. isStartPosition가 true면 입구쪽, false면 출구쪽
        if (isStartPosition)
            PlayerManager.Instance.SetPlayerPosition(currentMapData.playerSpawnPositions[0]);
        else
            PlayerManager.Instance.SetPlayerPosition(currentMapData.playerSpawnPositions[1]);
    }

    public void MoveToAnotherMap(MapType mapType, bool isStartPosition)
    {
        StartCoroutine(FadeOutAndMove(mapType, isStartPosition));
    }

    IEnumerator FadeOutAndMove(MapType mapType, bool isStartPosition)
    {
        if (screenFader == null)
            screenFader = UIManager.Instance.GetUI<ScreenFader>();

        screenFader.FadeOut();

        yield return new WaitForSeconds(1);

        LoadNewMap(mapType);

        LoadNewEntity(isStartPosition);

        screenFader.FadeIn();
    }

    void ClearMap()
    {
        if (currentMapData != null)
            currentMapData = null;
        if (mapInstance != null)
            Destroy(mapInstance.gameObject);
        if (currentWater != null)
            Destroy(currentWater.gameObject);
        if(currentMonsterList != null)
        {
            foreach (var obj in currentMonsterList)
            {
                if (obj != null)
                    Destroy(obj.gameObject);
            }
            currentMonsterList.Clear();
        }

    }


    // TODO : 맵 정보 가지고 있게 해주시면 됩니다. 
    // 로드 할 스테이지 번호 넣으면 페이드아웃 한 번 넣고 맵 프리팹 로드하시면 될 듯 합니다 
}
