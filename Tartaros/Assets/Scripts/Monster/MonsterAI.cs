using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterAI : MonoBehaviour
{
    [field : Header("AI Settings")]
    [field: SerializeField] public float MinX { get; set;} = -9f;
    [field: SerializeField] public float MaxX { get; set;} = 27f;
    
    [field: SerializeField] public Transform Target { get; set; }
    
    public Vector2 BasePosition { get; set; }
    public Vector2 SpawnPosition { get; private set; }
    
    public Vector2 Destination { get; set; }

    public bool isMoving = false;
    public bool isReturn = false;
    public bool isAttackDone = false;
    
    
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

    public abstract void Attack();

    public abstract void EndAttack();
    
    // 타겟 정하고 움직이는거
    public void MoveToTarget(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * Monster.data.MoveSpeed);
    }

    public void SetRandomDestination()
    {
        float newPosX = Mathf.Clamp(transform.position.x + Random.Range(-(Monster.data.Patrol / 2f), Monster.data.Patrol / 2f), MinX, MaxX);
        Destination = new Vector2(newPosX, transform.position.y);
    }
    
    // 목적지 정하고 움직이는거
    public bool MoveToDestination()
    {
        transform.position = Vector2.MoveTowards(transform.position, Destination, Time.deltaTime * Monster.data.MoveSpeed);
        isMoving = Vector2.Distance(transform.position, Destination) > Monster.data.DistanceThreshold;
        return !isMoving;
    }

    public abstract void HandleAnimationEvent(string eventName);

    
}
