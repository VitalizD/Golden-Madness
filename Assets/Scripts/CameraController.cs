using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instanse = null;

    [SerializeField] private bool enableMoving = true;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float yOffset = 1.6f;
    [SerializeField] private float upLimit;
    [SerializeField] private float bottomLimit;
    [SerializeField] private float leftLimit;
    [SerializeField] private float rightLimit;

    private Camera camera_;

    private Transform player;
    private Vector3 toPosition;
    private float zPosition;

    public bool EnableMoving { get => enableMoving; set => enableMoving = value; }

    public float Size { get => camera_.orthographicSize; set => camera_.orthographicSize = value; }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
            Destroy(gameObject);

        zPosition = transform.position.z;

        camera_ = GetComponent<Camera>();
    }

    private void Start()
    {    
        if (!player)
            player = Player.Instanse.transform;
    }

    private void Update()
    {
        if (enableMoving)
        {
            Move();
            FixedCamera();
        }

        //if (Input.GetButton("Vertical"))
        //{
        //    toPosition.y = player.position.y * Input.GetAxis("Vertical");
        //    transform.position = Vector3.Lerp(transform.position, toPosition, Time.deltaTime * speed);
        //    enableMoving = false;
        //}
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
        if (player != null)
        {
            toPosition = player.position;
            toPosition.y = player.position.y - yOffset;
            toPosition.z = zPosition;
            transform.position = Vector3.Lerp(transform.position, toPosition, Time.deltaTime * speed);
        }
    }
}
