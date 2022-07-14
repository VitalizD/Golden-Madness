using UnityEngine;

public class DoorFromSaveZone : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float cameraSize = 4.67f;
    [SerializeField] private float fadeSpeed = 1.2f;

    private Teleporter teleporter;
    private Hay hay;
    private Minecart minecart;
    private ChestSZ chest;
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
        CanBeUsed = true;
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
        chest = GameObject.FindGameObjectWithTag(ServiceInfo.ChestTag).GetComponent<ChestSZ>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Start()
    {
        normalCameraSize = CameraController.Instanse.Size;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canBeUsed && trigger.IsTriggered && teleporter.State == Teleporter.States.Stayed)
        {
            var fadeInLevelMusic = SceneMusic.Instanse.LevelMusic.GetComponent<Music>().MusicFade(false);
            var fadeOutSaveZoneMusic = SceneMusic.Instanse.SafeZoneMusic.GetComponent<Music>().MusicFade(true);
            StartCoroutine(fadeOutSaveZoneMusic);
            StartCoroutine(fadeInLevelMusic);
            void action()
            {
                //CameraController.Instanse.EnableMoving = true;
                CameraController.Instanse.Fix = false;
                CameraController.Instanse.transform.position = exitPosition;
                CameraController.Instanse.Size = normalCameraSize;
                Player.Instanse.GetComponent<SanityController>().DecreasingEnabled = true;
            }

            CanBeUsed = false;
            teleporter.Go(exitPosition, action, fadeSpeed);
        }
    }
}
