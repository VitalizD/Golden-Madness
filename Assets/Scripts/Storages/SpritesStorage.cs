using UnityEngine;
using System.Collections.Generic;

public class SpritesStorage : MonoBehaviour
{
    public static SpritesStorage Instanse { get; private set; } = null;

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
    [SerializeField] private Sprite rope;
    [SerializeField] private Sprite antidote;

    [Header("Tools")]
    [SerializeField] private Sprite pickaxe;
    [SerializeField] private Sprite lamp;

    [Header("Objects")]
    [SerializeField] private Sprite openedChest;

    [Header("Items")]
    [SerializeField] private Sprite firstArtifact;

    private Dictionary<ConsumableType, Sprite> consumables;
    private Dictionary<ResourceType, Sprite> resources;
    private Dictionary<EquipmentType, Sprite> equipments;

    public Sprite OpenedChest { get => openedChest; }

    public Sprite FirstArtifact { get => firstArtifact; }

    public Sprite GetConsumable(ConsumableType type) => consumables[type];

    public Sprite GetResource(ResourceType type) => resources[type];

    public Sprite GetEquipment(EquipmentType type) => equipments[type];

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        consumables = new Dictionary<ConsumableType, Sprite>
        {
            [ConsumableType.FuelTank] = fuelTank,
            [ConsumableType.Grindstone] = grindstone,
            [ConsumableType.HealthPack] = healthPack,
            [ConsumableType.SmokingPipe] = smokingPipe,
            [ConsumableType.Rope] = rope,
            [ConsumableType.Antidote] = antidote
        };

        resources = new Dictionary<ResourceType, Sprite>
        {
            [ResourceType.Coal] = coalIcon,
            [ResourceType.GoldOre] = goldIcon,
            [ResourceType.IronOre] = ironIcon,
            [ResourceType.Quartz] = quartzIcon
        };

        equipments = new Dictionary<EquipmentType, Sprite>
        {
            [EquipmentType.Pickaxe] = pickaxe,
            [EquipmentType.Lamp] = lamp
        };
    }
}
