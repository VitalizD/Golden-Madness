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

    [Header("Consumables")]
    [SerializeField] private TextMeshProUGUI fuelTanksCount;
    [SerializeField] private TextMeshProUGUI grindstonesCount;
    [SerializeField] private TextMeshProUGUI healthPacksCount;
    [SerializeField] private TextMeshProUGUI smokingPipesCount;
    [SerializeField] private TextMeshProUGUI dynamitesCount;
    [SerializeField] private TextMeshProUGUI antidotesCount;

    private Dictionary<ConsumableType, TextMeshProUGUI> consumablesCounts;
    private Dictionary<BarType, BarController> bars;

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
            [ConsumableType.Dynamite] = dynamitesCount,
            [ConsumableType.Antidote] = antidotesCount
        };

        bars = new Dictionary<BarType, BarController>
        {
            [BarType.Pickaxe] = pickaxeBar,
            [BarType.Lamp] = lampBar,
            [BarType.Health] = healthBar,
            [BarType.Sanity] = sanityBar
        };
    }
}
