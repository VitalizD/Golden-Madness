using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public void Spawn(RoomInfo[] rooms, LayerMask roomMask, float roomDetectionRadius)
    {
        var roomDetection = Physics2D.OverlapCircle(transform.position, roomDetectionRadius, roomMask);
        if (roomDetection == null)
        {
            Instantiate(rooms[Random.Range(0, rooms.Length)].gameObject, transform.position, Quaternion.identity);
        }
    }
}
