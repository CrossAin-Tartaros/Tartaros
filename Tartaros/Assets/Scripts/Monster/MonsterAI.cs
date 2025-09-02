using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterAI : MonoBehaviour
{
    [field : Header("AI Settings")]
    [field: SerializeField] public float Recognize { get; set;} = 8f;
    [field: SerializeField] public float Patrol { get; set;} = 10f;
    [field: SerializeField] public float PatrolWait { get; set;} = 2f;
    [field: SerializeField] public float AttackRange { get; set;} = 5f;
    [field: SerializeField] public float DistanceThreshold { get; set;} = 0.1f;
    [field: SerializeField] public float MinX { get; set;} = -9f;
    [field: SerializeField] public float MaxX { get; set;} = 27f;
    
    [field: SerializeField] public Transform Target { get; set; }
    
    public Vector2 BasePosition { get; set; }
    public Vector2 SpawnPosition { get; private set; }
    
    public Vector2 Destination { get; private set; }

    public bool isMoving = false;
    
    public Monster Monster { get; set; }
    protected BehaviourTree monsterTree;

    public void Init(Monster monster)
    {
        SpawnPosition = transform.position;
        BasePosition = SpawnPosition;
        this.Monster = monster;
        BuildBT();
    }
    
    private void Update()
    {
        monsterTree.Tick();
        
    }

    public abstract void BuildBT();
    
    
    // 타겟 정하고 움직이는거
    public void MoveToTarget(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * Monster.MoveSpeed);
    }

    public void SetRandomDestination()
    {
        float newPosX = Mathf.Clamp(transform.position.x + Random.Range(-(Patrol / 2f), Patrol / 2f), MinX, MaxX);
        Destination = new Vector2(newPosX, transform.position.y);
    }
    
    // 목적지 정하고 움직이는거
    public bool MoveToDestination()
    {
        transform.position = Vector2.MoveTowards(transform.position, Destination, Time.deltaTime * Monster.MoveSpeed);
        isMoving = Vector2.Distance(transform.position, Destination) > DistanceThreshold;
        return !isMoving;
    }
}
