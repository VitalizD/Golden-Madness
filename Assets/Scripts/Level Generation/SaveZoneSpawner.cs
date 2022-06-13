using UnityEngine;

public class SaveZoneSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public Transform GetRandomPoint() => spawnPoints[Random.Range(0, spawnPoints.Length)];
}
