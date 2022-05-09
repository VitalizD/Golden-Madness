using UnityEngine;

public class DoorFromSaveZone : MonoBehaviour
{
    [SerializeField] private Vector3 cameraPosition;

    private SanityController sanity;
    private Vector2 exitPosition;
    private bool isTriggered = false;

    public Vector3 CameraPosition { get => cameraPosition; set => cameraPosition = value; }

    public void SetExitPosition(Vector2 value) => exitPosition = value;

    private void Start()
    {
        sanity = Player.instanse.GetComponent<SanityController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            isTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            isTriggered = false;
    }

    private void Update()
    {
        if (isTriggered && Input.GetKeyDown(KeyCode.E))
        {
            Player.instanse.transform.position = exitPosition;
            CameraController.instanse.EnableMoving = true;
            CameraController.instanse.transform.position = exitPosition;
            sanity.DecreasingEnabled = true;
        }
    }
}
