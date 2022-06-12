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

    [Header("Rooms\n\n0 -> LR;\n1 -> LRB;\n2 -> LRT;\n3 -> LRTB")]
    [SerializeField] private GameObject[] rooms;
    [SerializeField] private GameObject[] entryRooms;
    [SerializeField] private GameObject[] exitRooms;

    private Direction[] directionsWithoutLeft;
    private Direction[] directionsWithoutRight;
    private Direction directionValue;
    private bool isGenerated = false;
    private bool onFirstRoom = true;
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
        GenerateRoom(GetRandomRoomFrom(entryRooms));
        directionValue = GetRandomDirectionFrom(directions);
        StartCoroutine(GenerateNext());
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
                onFirstRoom = false;
                transform.position = new Vector2(transform.position.x + moveAmount, transform.position.y);
                GenerateRoom(GetRandomRoomFrom(rooms));
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
                onFirstRoom = false;
                transform.position = new Vector2(transform.position.x - moveAmount, transform.position.y);
                GenerateRoom(GetRandomRoomFrom(rooms));
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
                var currentRoom = GetCurrentRoomInfo();
                if (currentRoom.Type != RoomDirection.LeftRightBottom && currentRoom.Type != RoomDirection.LeftRightTopBottom)
                {
                    currentRoom.Remove();
                    GameObject neededRoom;
                    if (bottomCounter >= 2)
                    {
                        neededRoom = rooms[3]; // LRTB
                    }
                    else
                    {
                        var neededIndexes = new[] { RoomDirection.LeftRightBottom, RoomDirection.LeftRightTopBottom }; // LRB, LRTB
                        var randomIndex = (int)neededIndexes[Random.Range(0, neededIndexes.Length)];
                        neededRoom = rooms[randomIndex];
                    }

                    if (!onFirstRoom)
                        GenerateRoom(neededRoom);
                    else
                        GenerateRoom(entryRooms[(int)neededRoom.GetComponent<RoomInfo>().Type]);
                }

                onFirstRoom = false;
                transform.position = new Vector2(transform.position.x, transform.position.y - moveAmount);
                GenerateRoom(rooms[Random.Range(2, 4)]); // LRT, LRTB
                directionValue = GetRandomDirectionFrom(directions);
            }
            else
            {
                var currentRoom = GetCurrentRoomInfo();
                currentRoom.Remove();
                GenerateRoom(exitRooms[(int)currentRoom.Type]);

                isGenerated = true;
                foreach (var roomSpawner in roomSpawners)
                    roomSpawner.Spawn(rooms, roomMask, roomDetectionRadius);
            }
        }
    }

    private Direction GetRandomDirectionFrom(Direction[] array) => array[Random.Range(0, array.Length)];

    private GameObject GetRandomRoomFrom(GameObject[] array) => array[Random.Range(0, array.Length)];

    private void GenerateRoom(GameObject room) => Instantiate(room, transform.position, Quaternion.identity);

    private RoomInfo GetCurrentRoomInfo() => Physics2D.OverlapCircle(transform.position, roomDetectionRadius, roomMask).GetComponent<RoomInfo>();

    private IEnumerator GenerateNext()
    {
        yield return new WaitForSeconds(timeBetweenRooms);
        Move();
        if (!isGenerated)
            StartCoroutine(GenerateNext());
    }
}
