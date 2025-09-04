using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterAI : MonoBehaviour
{
    [field : Header("AI Settings")]
    [field: SerializeField] public float MinX { get; set;} = -9f;
    [field: SerializeField] public float MaxX { get; set;} = 27f;
    
    public Transform Target { get; set; }
    
    public Vector2 BasePosition { get; set; }
    public Vector2 SpawnPosition { get; private set; }
    
    public Vector2 Destination { get; set; }

    public bool isMoving = false;
    public bool isReturn = false;
    public bool isAttackDone = false;
    public bool isPausedBT = false;
    public bool isStucked = false;
    public int stuckPosition = 0;
    
    
    public Monster Monster { get; set; }
    protected BehaviourTree monsterTree;
    private Vector2 targetPosition;

    public void Init(Monster monster)
    {
        SpawnPosition = transform.position;
        BasePosition = SpawnPosition;
        this.Monster = monster;
        Target = GameObject.FindWithTag("Player").transform;
        BuildBT();
    }
    
    private void Update()
    {
        if(!isPausedBT)
            monsterTree.Tick();
        
    }

    public abstract void Damaged();

    public abstract void BuildBT();
    

    public abstract void EndAttack();
    
    // 타겟 정하고 움직이는거
    public void MoveToTarget(Vector2 target)
    {
        targetPosition = new Vector2(target.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, Time.deltaTime * Monster.data.MoveSpeed);
    }

    public void SetRandomDestination()
    {
        float range;
        
        if (isStucked)
        {
            Debug.Log("Stucked New Position");
            if (stuckPosition < 0)
            {
                range = Random.Range(0, Monster.data.Patrol / 2f);
            }
            else
            {
                range = Random.Range(-(Monster.data.Patrol / 2f), 0);
            }

            isStucked = false;
            stuckPosition = 0;
        }
        else
        {
            range = Random.Range(-(Monster.data.Patrol / 2f), Monster.data.Patrol / 2f);
        }
        
        float newPosX = Mathf.Clamp(transform.position.x + range, MinX, MaxX);
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
