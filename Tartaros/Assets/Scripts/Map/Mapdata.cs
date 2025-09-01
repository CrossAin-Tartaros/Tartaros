using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapData", menuName = "Map/MapData")]

public class MapData : ScriptableObject
{
    public GameObject mapPrefab;

    // ���� ���� ������ ����Ʈ�� ����
    public List<MonsterSpawnData> monsterSpawnList;

    public List<Vector2> playerSpawnPositions;
}

[System.Serializable]
public struct MonsterSpawnData
{
    public GameObject monsterPrefab;
    public Vector2 position;
}