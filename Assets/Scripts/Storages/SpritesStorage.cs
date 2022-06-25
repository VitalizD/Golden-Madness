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
    [SerializeField] private Sprite dynamite;
    [SerializeField] private Sprite antidote;

    [Header("Tools")]
    [SerializeField] private Sprite pickaxe;
    [SerializeField] private Sprite lamp;

    private Dictionary<ConsumableType, Sprite> consumables;
    private Dictionary<ResourceTypes, Sprite> resources;
    private Dictionary<EquipmentType, Sprite> equipments;

    public Sprite GetConsumable(ConsumableType type) => consumables[type];

    public Sprite GetResource(ResourceTypes type) => resources[type];

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
            [ConsumableType.Dynamite] = dynamite,
            [ConsumableType.Antidote] = antidote
        };

        resources = new Dictionary<ResourceTypes, Sprite>
        {
            [ResourceTypes.Coal] = coalIcon,
            [ResourceTypes.GoldOre] = goldIcon,
            [ResourceTypes.IronOre] = ironIcon,
            [ResourceTypes.Quartz] = quartzIcon
        };

        equipments = new Dictionary<EquipmentType, Sprite>
        {
            [EquipmentType.Pickaxe] = pickaxe,
            [EquipmentType.Lamp] = lamp
        };
    }
}
