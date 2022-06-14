using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] private RoomDirection type;
    [SerializeField] private Transform spawnPointPlayer;

    public RoomDirection Type { get => type; }

    public Transform SpawnPointPlayer { get => spawnPointPlayer; }

    public void Remove() => Destroy(gameObject);
}
