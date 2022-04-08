using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 position;

    private void Awake()
    {
        if (!player)
            player = FindObjectOfType<Player>().transform;
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
