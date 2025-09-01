using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    private GameObject mapInstance;

    //Constans/MapName 클래스에서 Resources 폴더 내 경로를 상수로 저장해서 사용.
    //public const string stage1 = "Maps/stage1(or 다른 이름)"


    void LoadNewMap(string mapName, bool isStartPosition)
    {
        
        if(mapInstance != null)
            Destroy(mapInstance.gameObject);

        //기본 구조물 로드
        mapInstance = Instantiate(Resources.Load<MapData>("Maps/" + mapName).mapPrefab);

        //플레이어 위치 설정. isStartPosition가 true면 입구쪽, false면 출구쪽
        if (isStartPosition) 
        { }
        else 
        { }

        //몬스터 소환

        //페이드 인 효과

        if (mapInstance != null)
            Debug.Log($"{mapName} 로드");
    }

    public void MoveToAnotherMap(string mapName, bool isStartPosition)
    {
        //페이드 아웃

        LoadNewMap(mapName, isStartPosition);
    }


    // TODO : 맵 정보 가지고 있게 해주시면 됩니다. 
    // 로드 할 스테이지 번호 넣으면 페이드아웃 한 번 넣고 맵 프리팹 로드하시면 될 듯 합니다 
}
