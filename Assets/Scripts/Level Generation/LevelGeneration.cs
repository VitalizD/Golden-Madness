using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class LevelGeneration : MonoBehaviour
{
    public static LevelGeneration Instanse { get; private set; } = null;

    [Header("Bounds")]
    [SerializeField] private int minX;
    [SerializeField] private int maxX;
    [SerializeField] private int minY;

    [Header("Ore Spawn Settings")]
    [SerializeField] [Range(0f, 1f)] private float spawnOreChance = 0.15f;
    [SerializeField] private GameObject[] oresPrefabs;

    [Tooltip("Укажите части (например, 1, 1, 2)")]
    [SerializeField] private float[] spawnChances;

    [Space]

    [SerializeField] private int moveAmount;
    [SerializeField] private float timeBetweenRooms = 0.25f;
    [SerializeField] private float roomDetectionRadius = 1f;
    [SerializeField] private LayerMask roomMask;
    [SerializeField] private Direction[] directions;
    [SerializeField] private Transform[] startPositions;
    [SerializeField] private RoomSpawner[] roomSpawners;

    // Indexes: 0 -> LR; 1 -> LRB; 2 -> LRT; 3 -> LRTB
    [SerializeField] private GameObject[] rooms;

    private Direction[] directionsWithoutLeft;
    private Direction[] directionsWithoutRight;
    private Direction directionValue;
    private bool isGenerated = false;
    private int bottomCounter = 0;

    public bool IsGenerated { get => isGenerated; }

    public float SpawnOreChance { get => spawnOreChance; }

    public GameObject[] OrePrefabs { get => oresPrefabs; }

    public float[] SpawnChances { get => spawnChances; }

    private void Awake()
    {
        if (oresPrefabs.Length != spawnChances.Length)
            throw new System.Exception("Размеры массивов \"Spawn Chances\" и \"Ores Prefabs\" не совпадают");

        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

        directionsWithoutLeft = directions.Where(dir => dir != Direction.Left).ToArray();
        directionsWithoutRight = directions.Where(dir => dir != Direction.Right).ToArray();
    }

    private void Start()
    {
        transform.position = startPositions[Random.Range(0, startPositions.Length)].position;
        Instantiate(GetRandomRoom(), transform.position, Quaternion.identity);
        directionValue = GetRandomDirectionFrom(directions);
        StartCoroutine(GenerateRoom());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, roomDetectionRadius);
    }

    private void Move()
    {
        if (directionValue == Direction.Right) // Move right
        {
            if (transform.position.x < maxX)
            {
                bottomCounter = 0;
                transform.position = new Vector2(transform.position.x + moveAmount, transform.position.y);
                Instantiate(GetRandomRoom(), transform.position, Quaternion.identity);
                directionValue = GetRandomDirectionFrom(directionsWithoutLeft);
            }
            else
                directionValue = Direction.Bottom;
        }
        else if (directionValue == Direction.Left) // Move left
        {
            if (transform.position.x > minX)
            {
                bottomCounter = 0;
                transform.position = new Vector2(transform.position.x - moveAmount, transform.position.y);
                Instantiate(GetRandomRoom(), transform.position, Quaternion.identity);
                directionValue = GetRandomDirectionFrom(directionsWithoutRight);
            }
            else
                directionValue = Direction.Bottom;
        }
        else if (directionValue == Direction.Bottom) // Move bottom
        {
            ++bottomCounter;

            if (transform.position.y > minY)
            {
                var roomDetection = Physics2D.OverlapCircle(transform.position, roomDetectionRadius, roomMask);
                var room = roomDetection.GetComponent<RoomInfo>();
                if (room.Type != RoomDirection.LeftRightBottom && room.Type != RoomDirection.LeftRightTopBottom)
                {
                    if (bottomCounter >= 2)
                    {
                        room.Remove();
                        Instantiate(rooms[3], transform.position, Quaternion.identity);
                    }
                    else
                    {
                        room.Remove();
                        var neededIndexes = new[] { RoomDirection.LeftRightBottom, RoomDirection.LeftRightTopBottom };
                        var randomIndex = (int)neededIndexes[Random.Range(0, neededIndexes.Length)];
                        Instantiate(rooms[randomIndex], transform.position, Quaternion.identity);
                    }
                }

                transform.position = new Vector2(transform.position.x, transform.position.y - moveAmount);
                Instantiate(rooms[Random.Range(2, 4)], transform.position, Quaternion.identity);
                directionValue = GetRandomDirectionFrom(directions);
            }
            else
            {
                isGenerated = true;
                foreach (var roomSpawner in roomSpawners)
                    roomSpawner.Spawn(rooms, roomMask, roomDetectionRadius);
            }
        }
    }

    private Direction GetRandomDirectionFrom(Direction[] array) => array[Random.Range(0, array.Length)];

    private GameObject GetRandomRoom() => rooms[Random.Range(0, rooms.Length)];

    private IEnumerator GenerateRoom()
    {
        Move();
        yield return new WaitForSeconds(timeBetweenRooms);
        if (!isGenerated)
            StartCoroutine(GenerateRoom());
    }
}
