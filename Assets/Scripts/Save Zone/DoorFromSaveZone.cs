using UnityEngine;

public class DoorFromSaveZone : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 1.2f;

    private Teleporter teleporter;
    private Hay hay;
    private Minecart minecart;
    private Chest chest;

    private Vector3 cameraPosition;
    private Vector2 exitPosition;
    private bool isTriggered = false;

    public Vector3 CameraPosition { get => cameraPosition; set => cameraPosition = value; }

    public bool CanBeUsed { get => canBeUsed; set => canBeUsed = value; }

    public void Refresh(Vector2 exitPosition)
    {
        this.exitPosition = exitPosition;
        hay.CanBeUsed = true;
        minecart.gameObject.SetActive(true);
        chest.CanBeUsed = true;
    }

    private void Awake()
    {
        var center = transform.parent.transform.position;
        cameraPosition = new Vector3(center.x, center.y, -10);

        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.SceneControllerTag).GetComponent<Teleporter>();
        hay = GameObject.FindGameObjectWithTag(ServiceInfo.HayTag).GetComponent<Hay>();
        minecart = GameObject.FindGameObjectWithTag(ServiceInfo.MinecartTag).GetComponent<Minecart>();
        chest = GameObject.FindGameObjectWithTag(ServiceInfo.ChestTag).GetComponent<Chest>();
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
        if (Input.GetKeyDown(KeyCode.E) && canBeUsed && isTriggered && teleporter.State == Teleporter.States.Stayed)
        {
            void action()
            {
                CameraController.instanse.EnableMoving = true;
                CameraController.instanse.transform.position = exitPosition;
                Player.instanse.GetComponent<SanityController>().DecreasingEnabled = true;
            }

            teleporter.Go(exitPosition, action, fadeSpeed);
        }
    }
}
