using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    public static CameraController Instanse { get; private set; } = null;

    [SerializeField] private bool enableMoving = true;
    [SerializeField] private bool fix = false;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float yOffset = 1.6f;
    [SerializeField] private float lookUpOrDown = 3f;
    [SerializeField] private float upLimit;
    [SerializeField] private float bottomLimit;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;

    private Camera camera_;

    private Transform player;
    private Vector3 toPosition;
    private float zPosition;

    public bool EnableMoving { get => enableMoving; set => enableMoving = value; }

    public bool Fix { get => fix; set => fix = value; }

    public float Size { get => camera_.orthographicSize; set => camera_.orthographicSize = value; }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

        zPosition = transform.position.z;

        camera_ = GetComponent<Camera>();
    }

    private void Start()
    {    
        if (!player)
            player = Player.Instanse.transform;
    }

    private void FixedUpdate()
    {
        if (enableMoving)
        {
            Move();
            if (fix)
                FixedCamera();
        }
    }

    private void OnDrawGizmosSelected()
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
            zPosition
        );
    }

    private void Move()
    {
        if (player == null)
            return;

        toPosition = player.position;
        toPosition.y = player.position.y - yOffset;
        toPosition.z = zPosition;

        if (!Player.Instanse.IsClimbing && Player.Instanse.CanJump)
            toPosition.y += lookUpOrDown * Input.GetAxis("Vertical");

        transform.position = Vector3.Lerp(transform.position, toPosition, Time.fixedDeltaTime * speed);
    }
}
