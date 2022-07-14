using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HotbarController : MonoBehaviour
{
    public static HotbarController Instanse { get; private set; } = null;

    [Header("Bars")]
    [SerializeField] private BarController pickaxeBar;
    [SerializeField] private BarController lampBar;
    [SerializeField] private BarController healthBar;
    [SerializeField] private BarController sanityBar;

    [Header("Level Texts")]
    [SerializeField] private TextMeshProUGUI pickaxeLevel;
    [SerializeField] private TextMeshProUGUI lampLevel;

    [Header("Consumables")]
    [SerializeField] private TextMeshProUGUI fuelTanksCount;
    [SerializeField] private TextMeshProUGUI grindstonesCount;
    [SerializeField] private TextMeshProUGUI healthPacksCount;
    [SerializeField] private TextMeshProUGUI smokingPipesCount;
    [SerializeField] private TextMeshProUGUI ropesCount;
    [SerializeField] private TextMeshProUGUI antidotesCount;

    private Dictionary<ConsumableType, TextMeshProUGUI> consumablesCounts;
    private Dictionary<BarType, BarController> bars;
    private Dictionary<EquipmentType, TextMeshProUGUI> equipmentLevels;

    public void Load()
    {
        SetEquipmentLevel(EquipmentType.Pickaxe, PlayerPrefs.GetInt(BuildingType.Forge.ToString() + PlayerPrefsKeys.CurrentLevelOfBuildingPostfix, 1));
        SetEquipmentLevel(EquipmentType.Lamp, PlayerPrefs.GetInt(BuildingType.Workshow.ToString() + PlayerPrefsKeys.CurrentLevelOfBuildingPostfix, 1));
    }

    public void SetConsumableCount(ConsumableType type, int value)
    {
        consumablesCounts[type].text = value.ToString();
    }

    public void AddConsumableCount(ConsumableType type, int value)
    {
        var newValue = int.Parse(consumablesCounts[type].text) + value;
        consumablesCounts[type].text = newValue.ToString();
    }

    public void SetBarValue(BarType type, float value)
    {
        bars[type].SetValue(value);
    }

    public void AddBarValue(BarType type, float value)
    {
        var currentValue = bars[type].GetValue();
        bars[type].SetValue(currentValue + value);
    }

    public void SetEquipmentLevel(EquipmentType type, int value)
    {
        if (value < 1)
            value = 1;

        equipmentLevels[type].text = $"Óð. {value}";
    }

    public void UpdateConsumablesCount(Dictionary<ConsumableType, int> counts)
    {
        foreach (var element in counts)
            SetConsumableCount(element.Key, element.Value);
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

        consumablesCounts = new Dictionary<ConsumableType, TextMeshProUGUI>
        {
            [ConsumableType.FuelTank] = fuelTanksCount,
            [ConsumableType.Grindstone] = grindstonesCount,
            [ConsumableType.HealthPack] = healthPacksCount,
            [ConsumableType.SmokingPipe] = smokingPipesCount,
            [ConsumableType.Rope] = ropesCount,
            [ConsumableType.Antidote] = antidotesCount
        };

        bars = new Dictionary<BarType, BarController>
        {
            [BarType.Pickaxe] = pickaxeBar,
            [BarType.Lamp] = lampBar,
            [BarType.Health] = healthBar,
            [BarType.Sanity] = sanityBar
        };

        equipmentLevels = new Dictionary<EquipmentType, TextMeshProUGUI>
        {
            [EquipmentType.Pickaxe] = pickaxeLevel,
            [EquipmentType.Lamp] = lampLevel
        };
    }

    private void Start()
    {
        Load();
    }
}
