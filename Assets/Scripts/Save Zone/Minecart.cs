using UnityEngine;

public class Minecart : MonoBehaviour
{
    private const string leaveAnimationName = "Leave";
    private const string returnAnimationName = "Return";

    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private GameObject minecart;
    [SerializeField] private SFX minecartSFX;

    private Animation minecartAnimation;
    private TriggerZone trigger;
    private PressActionKey pressActionKey;

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);

            minecartAnimation.Play(returnAnimationName);
            if (!value)
                minecartAnimation.Play(leaveAnimationName);
        }
    }

    private void Awake()
    {
        minecartAnimation = minecart.GetComponent<Animation>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Update()
    {
        if (trigger.IsTriggered && Input.GetKeyDown(KeyCode.E) && canBeUsed)
        {
            var backpack = Player.Instanse.GetComponent<Backpack>();
            ResourcesSaver.AddInVillage(backpack.GetAll());
            backpack.Clear();
            minecartSFX.Play();
            StartCoroutine(minecartSFX.SoundFade(2.5f));
            // Для обучающего уровня
            ServiceInfo.CheckpointConditionDone = true;

            CanBeUsed = false;
        }
    }
}
