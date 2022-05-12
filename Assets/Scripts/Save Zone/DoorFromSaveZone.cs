using UnityEngine;

public class DoorFromSaveZone : MonoBehaviour
{
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private float stunTime = 2f;

    private Vector2 exitPosition;
    private Teleporter teleporter;
    private bool isTriggered = false;

    public Vector3 CameraPosition { get => cameraPosition; set => cameraPosition = value; }

    public void SetExitPosition(Vector2 value) => exitPosition = value;

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.SceneControllerTag).GetComponent<Teleporter>();
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
        if (Input.GetKeyDown(KeyCode.E) && isTriggered && teleporter.State == Teleporter.States.Stayed)
        {
            void action()
            {
                CameraController.instanse.EnableMoving = true;
                CameraController.instanse.transform.position = exitPosition;
                Player.instanse.GetComponent<SanityController>().DecreasingEnabled = true;
            }

            Player.instanse.SetStun(stunTime);
            teleporter.Go(exitPosition, action);
        }
    }
}
