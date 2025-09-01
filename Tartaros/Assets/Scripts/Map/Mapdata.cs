using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapData", menuName = "Map/MapData")]

public class MapData : ScriptableObject
{
    public GameObject mapPrefab;

    // 몬스터 스폰 정보를 리스트로 관리
    public List<MonsterSpawnData> monsterSpawnList;

    public List<Vector2> playerSpawnPositions;
}

[System.Serializable]
public struct MonsterSpawnData
{
    public GameObject monsterPrefab;
    public Vector2 position;
}