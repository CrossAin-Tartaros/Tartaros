using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Monster/New Monster Data")]
public class MonsterData : ScriptableObject
{
    [field: Header("Sound Settings")] 
    [field: SerializeField] public AudioClip DamagedSound;
    [field: SerializeField] public AudioClip AttackSound;
    
    [field : Header("Stats")]
    [field: SerializeField] public MonsterType MonsterType { get; set; }
    [field: SerializeField] public int MaxHealth { get; set;} = 100;
    [field: SerializeField] public float MoveSpeed { get; set;} = 2f;
    
    [field: SerializeField] public int AttackDamage { get; set;} = 10;
    
    [field: SerializeField] public GameObject[] DropItems { get; set;}
    
    [field : Header("AI Settings")]
    [field: SerializeField] public float Recognize { get; set;} = 8f;
    [field: SerializeField] public float Patrol { get; set;} = 10f;
    [field: SerializeField] public float PatrolWait { get; set;} = 2f;
    [field: SerializeField] public float MaxBaseDistance { get; set;} = 10f;
    
    [field: SerializeField] public float AttackRange { get; set;} = 5f;
    [field: SerializeField] public float AttackWait { get; set;} = 0.5f;
    [field: SerializeField] public float StunWait { get; set;} = 0.5f;
    [field: SerializeField] public float StunKnockBack { get; set;} = 2f;
    [field: SerializeField] public float DistanceThreshold { get; set;} = 0.1f;
}
