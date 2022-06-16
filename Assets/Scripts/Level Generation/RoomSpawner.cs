using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public void Spawn(GameObject[] rooms, LayerMask roomMask, float roomDetectionRadius)
    {
        var roomDetection = Physics2D.OverlapCircle(transform.position, roomDetectionRadius, roomMask);
        if (roomDetection == null)
        {
            Instantiate(rooms[Random.Range(0, rooms.Length)], transform.position, Quaternion.identity);
        }
    }
}
