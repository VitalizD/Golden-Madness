using UnityEngine;

public class DoorFromSaveZone : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float cameraSize = 4.67f;
    [SerializeField] private float fadeSpeed = 1.2f;

    private Teleporter teleporter;
    private Hay hay;
    private Minecart minecart;
    private Chest chest;
    private TriggerZone trigger;
    private PressActionKey pressActionKey;

    private Vector3 cameraPosition;
    private Vector2 exitPosition;
    private float normalCameraSize;

    public Vector3 CameraPosition { get => cameraPosition; set => cameraPosition = value; }

    public float CameraSizeInSaveZone { get => cameraSize; }

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);
        }
    }

    public void Refresh(Vector2 exitPosition)
    {
        this.exitPosition = exitPosition;
        hay.CanBeUsed = true;
        minecart.CanBeUsed = true;
        chest.CanBeUsed = true;
    }

    private void Awake()
    {
        var center = transform.parent.transform.position;
        cameraPosition = new Vector3(center.x, center.y, -10);

        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag).GetComponent<Teleporter>();
        hay = GameObject.FindGameObjectWithTag(ServiceInfo.HayTag).GetComponent<Hay>();
        minecart = GameObject.FindGameObjectWithTag(ServiceInfo.MinecartTag).GetComponent<Minecart>();
        chest = GameObject.FindGameObjectWithTag(ServiceInfo.ChestTag).GetComponent<Chest>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Start()
    {
        normalCameraSize = CameraController.instanse.Size;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canBeUsed && trigger.IsTriggered && teleporter.State == Teleporter.States.Stayed)
        {
            void action()
            {
                CameraController.instanse.EnableMoving = true;
                CameraController.instanse.transform.position = exitPosition;
                CameraController.instanse.Size = normalCameraSize;
                Player.instanse.GetComponent<SanityController>().DecreasingEnabled = true;
            }

            CanBeUsed = false;
            teleporter.Go(exitPosition, action, fadeSpeed);
        }
    }
}
