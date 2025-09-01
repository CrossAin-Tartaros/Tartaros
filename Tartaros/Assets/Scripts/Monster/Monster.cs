using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    
    // TODO : 나중에 Entity로 관리해야함
    
    [field : Header("Stats")]
    [field: SerializeField] public MonsterType MonsterType { get; set; }
    [field: SerializeField] public int MaxHealth { get; set;} = 100;
    [field: SerializeField] public float MoveSpeed { get; set;} = 2f;
    public int CurrentHealth { get; set; }
    
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

    private BehaviourTree monsterTree;

    public bool isMoving = false;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
        SpawnPosition = transform.position;
        BasePosition = SpawnPosition;
    }

    private void Start()
    {
        BuildBT();
    }

    private void Update()
    {
        monsterTree.Tick();
    }

    public void Die()
    {
        // TODO : 죽으면 진행
    }

    private void BuildBT()
    {
        Node isDeathNode = new IsDeathNode(this);

        Node canRecogPlayerNode = new CanRecogPlayerNode(this, Target);
        Node chasePlayerNode = new ChasePlayerNode(this, Target);
        Sequence chaseSequence = new Sequence(new List<Node>
        {
            canRecogPlayerNode, chasePlayerNode
        });

        Node getRandomPosNode = new GetRandomPositionNode(this);
        Node moveNode = new MoveNode(this);
        Node patrolWaitNode = new WaitNode(PatrolWait);

        Sequence patrolSequence = new Sequence(new List<Node>
        {
            getRandomPosNode, moveNode, patrolWaitNode
        });

        Selector mainSelector = new Selector(new List<Node>
        {
            isDeathNode,
            chaseSequence,
            patrolSequence
        });
        
        monsterTree = new BehaviourTree(mainSelector);
    }

    public void MoveToTarget(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * MoveSpeed);
    }

    public void SetRandomDestination()
    {
        float newPosX = Mathf.Clamp(transform.position.x + Random.Range(-(Patrol / 2f), Patrol / 2f), MinX, MaxX);
        Destination = new Vector2(newPosX, transform.position.y);
    }
    
    public bool MoveToDestination()
    {
        transform.position = Vector2.MoveTowards(transform.position, Destination, Time.deltaTime * MoveSpeed);
        isMoving = Vector2.Distance(transform.position, Destination) > DistanceThreshold;
        return !isMoving;
    }
}
