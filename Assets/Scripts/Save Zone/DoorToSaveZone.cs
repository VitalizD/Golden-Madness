using UnityEngine;
using Agava.YandexGames;
using Utils;

public class DoorToSaveZone : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 1.2f;
    [SerializeField] private DoorFromSaveZone doorFromSaveZone;
    [SerializeField] private Sprite blockedDoorSprite;

    private Teleporter teleporter;
    private TriggerZone trigger;
    private PressActionKey pressActionKey;
    private SpriteRenderer sprite;

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);
        }
    }

    public void SetDoorFromSaveZone(DoorFromSaveZone value) => doorFromSaveZone = value;

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag).GetComponent<Teleporter>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (LevelGeneration.Instanse == null)
            return;

        doorFromSaveZone = LevelGeneration.Instanse.DoorFromSaveZone;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canBeUsed && trigger.IsTriggered && teleporter.State == Teleporter.States.Stayed)
        {
            var fadeOutLevelMusic = SceneMusic.Instanse.LevelMusic.GetComponent<Music>().MusicFade(true);
            var fadeInSaveZoneMusic= SceneMusic.Instanse.SafeZoneMusic.GetComponent<Music>().MusicFade(false);
            StartCoroutine(fadeOutLevelMusic);
            StartCoroutine(fadeInSaveZoneMusic);
            void action()
            {
                //CameraController.Instanse.EnableMoving = false;
                CameraController.Instanse.Fix = true;
                CameraController.Instanse.transform.position = doorFromSaveZone.CameraPosition;
                CameraController.Instanse.Size = doorFromSaveZone.CameraSizeInSaveZone;
                Player.Instanse.GetComponent<SanityController>().DecreasingEnabled = false;
                sprite.sprite = blockedDoorSprite;
            }

            CanBeUsed = false;
            doorFromSaveZone.Refresh(transform.position);
            teleporter.Go(doorFromSaveZone.transform.position, action, fadeSpeed);

            InterestialAd.Show();
            
        }
    }
}
