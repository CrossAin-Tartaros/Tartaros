using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    public MapData Data;
    public Transform[] monsterSpawnPoints; // 몬스터가 생성될 위치들
    public Transform[] playerStartPoints = new Transform[2];     // 플레이어가 시작할 위치
}
