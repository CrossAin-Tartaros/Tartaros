using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private Dictionary<string, MapData> mapDatas = new();
    private MapData currentMapData;
    private GameObject mapInstance;

    private GameObject waterPrefab;
    private Water currentWater;

    private List<GameObject> currentMonsterList = new();

    //Enum/MapType에서 Resources 폴더 내 Mapdata 이름을 저장해서 사용.
    private void Awake()
    {
        mapDatas.Add(MapType.Test.ToString(), Resources.Load<MapData>("Maps/" + MapType.Test.ToString()));
        mapDatas.Add(MapType.Stage1.ToString(), Resources.Load<MapData>("Maps/" + MapType.Stage1.ToString()));
        mapDatas.Add(MapType.Boss.ToString(), Resources.Load<MapData>("Maps/" + MapType.Boss.ToString()));

        waterPrefab = Resources.Load<GameObject>("Maps/" + "Fontaine");
    }


    void LoadNewMap(string mapName, bool isStartPosition)
    {
        //초기화
        ClearMap();

        currentMapData = mapDatas[mapName];

        //기본 구조물 로드
        mapInstance = Instantiate(currentMapData.mapPrefab);

        //샘물 로드
        if (currentMapData.waterPosition != Vector2.zero)
        {
            currentWater = Instantiate(waterPrefab, currentMapData.waterPosition, Quaternion.identity).gameObject.GetComponent<Water>();

            //저장 데이터에 따라서 샘물 사용여부 결정
            //if(사용했다면)
            currentWater.SetUsedWater();
        }


        //플레이어 위치 설정. isStartPosition가 true면 입구쪽, false면 출구쪽
        if (isStartPosition) 
        { }
        else 
        { }

        //몬스터 소환
        for (int i = 0; i < currentMapData.monsterSpawnList.Count; i++) 
        {
            
            GameObject obj = Instantiate(currentMapData.monsterSpawnList[i].monsterPrefab, currentMapData.monsterSpawnList[i].position, Quaternion.identity);
            currentMonsterList.Add(obj);
        }

        //페이드 인 효과

        if (mapInstance != null)
            Debug.Log($"{mapName} 로드");
    }

    public void MoveToAnotherMap(string mapName, bool isStartPosition)
    {
        //페이드 아웃

        LoadNewMap(mapName, isStartPosition);
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
