using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    public MapData Data;
    public Transform[] monsterSpawnPoints; // ���Ͱ� ������ ��ġ��
    public Transform[] playerStartPoints = new Transform[2];     // �÷��̾ ������ ��ġ
}
