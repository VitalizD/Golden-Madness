using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float yOffset = 1.6f;
    [SerializeField] private float upLimit;
    [SerializeField] private float bottomLimit;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;

    private Transform player;
    private Vector3 toPosition;

    private void Start()
    {
        if (!player)
            player = Player.instanse.transform;
    }

    private void Update()
    {
        Move();
        FixedCamera();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(leftLimit, upLimit), new Vector2(rightLimit, upLimit));
        Gizmos.DrawLine(new Vector2(leftLimit, bottomLimit), new Vector2(rightLimit, bottomLimit));
        Gizmos.DrawLine(new Vector2(leftLimit, upLimit), new Vector2(leftLimit, bottomLimit));
        Gizmos.DrawLine(new Vector2(rightLimit, upLimit), new Vector2(rightLimit, bottomLimit));
    }

    private void FixedCamera()
    {
        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
            Mathf.Clamp(transform.position.y, bottomLimit, upLimit),
            transform.position.z
        );
    }

    private void Move()
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
