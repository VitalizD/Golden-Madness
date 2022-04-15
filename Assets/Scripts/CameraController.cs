using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform player;
    private Vector3 position;

    private void Start()
    {
        if (!player)
            player = Player.instanse.transform;
    }

    private void Update()
    {
        if (player != null)
        {
            position = player.position;
            position.z = -10;
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime);
        }
    }
}
