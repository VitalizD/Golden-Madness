using UnityEngine;

public class DoorFromSaveZone : MonoBehaviour
{
    [SerializeField] private Vector3 cameraPosition;

    private readonly string playerLayer = "Player";

    private SanityController sanity;
    private Vector2 exitPosition;

    public Vector3 CameraPosition { get => cameraPosition; set => cameraPosition = value; }

    public void SetExitPosition(Vector2 value) => exitPosition = value;

    private void Start()
    {
        sanity = Player.instanse.GetComponent<SanityController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E) && collision.CompareTag(playerLayer))
        {
            collision.transform.position = exitPosition;
            CameraController.instanse.EnableMoving = true;
            CameraController.instanse.transform.position = exitPosition;
            sanity.DecreasingEnabled = true;
        }
    }
}
