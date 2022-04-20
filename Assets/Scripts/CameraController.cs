using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float yOffset = 1.6f;

    private Transform player;
    private Vector3 toPosition;

    private void Start()
    {
        if (!player)
            player = Player.instanse.transform;
    }

    private void Update()
    {
        if (player != null)
        {
            toPosition = player.position;
            toPosition.y = player.position.y - yOffset;
            toPosition.z = -10;
            transform.position = Vector3.Lerp(transform.position, toPosition, Time.deltaTime);
        }
    }
}
