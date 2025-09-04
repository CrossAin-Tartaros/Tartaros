using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossMonsterData", menuName = "Monster/New Boss Monster Data")]
public class BossMonsterData : MonsterData
{
    [field: Header("Boss Additional Settings")] 
    [field: SerializeField] public AudioClip RangeAttackSound;
}
