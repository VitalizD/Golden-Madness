using UnityEngine;

public class SpritesStorage : MonoBehaviour
{
    public static SpritesStorage instanse = null;

    [Header("Resources")]
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite coalIcon;
    [SerializeField] private Sprite quartzIcon;
    [SerializeField] private Sprite ironIcon;

    [Header("Consumables")]
    [SerializeField] private Sprite fuelTank;
    [SerializeField] private Sprite grindstone;
    [SerializeField] private Sprite healthPack;
    [SerializeField] private Sprite smokingPipe;
    [SerializeField] private Sprite dynamite;
    [SerializeField] private Sprite antidote;

    public Sprite GoldIcon { get => goldIcon; }

    public Sprite CoalIcon { get => coalIcon; }

    public Sprite QuartzIcon { get => quartzIcon; }

    public Sprite IronIcon { get => ironIcon; }

    public Sprite FuelTank { get => fuelTank; }

    public Sprite Grindstone { get => grindstone; }

    public Sprite HealthPack { get => healthPack; }

    public Sprite SmokingPipe { get => smokingPipe; }

    public Sprite Dynamite { get => dynamite; }

    public Sprite Antidote { get => antidote; }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
