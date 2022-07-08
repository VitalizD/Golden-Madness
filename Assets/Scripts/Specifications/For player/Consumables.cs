using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Consumables : MonoBehaviour, IStorage
{
    [Header("Counts")]
    [SerializeField] private int fuelTanksCount = 1;
    [SerializeField] private int grindstonesCount = 1;
    [SerializeField] private int healthPacksCount = 1;
    [SerializeField] private int smokingPipesCount = 1;

    [Header("Recoveries")]
    [SerializeField] private float fuelTankRecovery = 30f;
    [SerializeField] private float grindstoneRecovery = 30f;
    [SerializeField] private float healthPackRecovery = 30f;
    [SerializeField] private float smokingPipeRecovery = 30f;

    private Dictionary<ConsumableType, int> consumableCounts;
    private Dictionary<ConsumableType, string> consumableNames;
    private Dictionary<ConsumableType, float> consumableRecoveries;
    private bool loaded = false;

    public void Save()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.FuelTanksCount, fuelTanksCount);
        PlayerPrefs.SetInt(PlayerPrefsKeys.GrindstonesCount, grindstonesCount);
        PlayerPrefs.SetInt(PlayerPrefsKeys.HealthPacksCount, healthPacksCount);
        PlayerPrefs.SetInt(PlayerPrefsKeys.SmokingPipesCount, smokingPipesCount);
    }

    public void Load()
    {
        consumableCounts = new Dictionary<ConsumableType, int>
        {
            [ConsumableType.FuelTank] = PlayerPrefs.GetInt(PlayerPrefsKeys.FuelTanksCount, fuelTanksCount),
            [ConsumableType.Grindstone] = PlayerPrefs.GetInt(PlayerPrefsKeys.GrindstonesCount, grindstonesCount),
            [ConsumableType.HealthPack] = PlayerPrefs.GetInt(PlayerPrefsKeys.HealthPacksCount, healthPacksCount),
            [ConsumableType.SmokingPipe] = PlayerPrefs.GetInt(PlayerPrefsKeys.SmokingPipesCount, smokingPipesCount),
            [ConsumableType.Dynamite] = PlayerPrefs.GetInt(PlayerPrefsKeys.DynamitesCount, 0),
            [ConsumableType.Antidote] = PlayerPrefs.GetInt(PlayerPrefsKeys.AntidotesCount, 0)
        };
        UpdateHotbar();
        loaded = true;
    }

    public void Add(ConsumableType type, int count)
    {
        var initialValue = consumableCounts[type];

        if (consumableCounts[type] + count < 0)
            consumableCounts[type] = 0;
        else consumableCounts[type] += count;

        if (HotbarController.Instanse != null)
            HotbarController.Instanse.SetConsumableCount(type, consumableCounts[type]);

        if (consumableCounts[type] - initialValue > 0 && loaded && SceneManager.GetActiveScene().name != ServiceInfo.VillageScene)
            TextMessagesQueue.Instanse.Add($"{consumableNames[type]} x{consumableCounts[type] - initialValue}", SpritesStorage.Instanse.GetConsumable(type), 1f);
    }

    public int GetCount(ConsumableType type) => consumableCounts[type];

    public float GetRecovery(ConsumableType type) => consumableRecoveries[type];

    public void SetDefaultValues()
    {
        consumableCounts = new Dictionary<ConsumableType, int>
        {
            [ConsumableType.FuelTank] = 1,
            [ConsumableType.Grindstone] = 1,
            [ConsumableType.HealthPack] = 1,
            [ConsumableType.SmokingPipe] = 1,
            [ConsumableType.Dynamite] = 1,
            [ConsumableType.Antidote] = 1
        };
        UpdateHotbar();
    }

    public void SetFuelTanksCount(int value) => Add(ConsumableType.FuelTank, value);

    public void SetGrindstonesCount(int value) => Add(ConsumableType.Grindstone, value);

    public void SetHealthPacksCount(int value) => Add(ConsumableType.HealthPack, value);

    public void SetSmokingPipesCount(int value) => Add(ConsumableType.SmokingPipe, value);

    private void Awake()
    {
        consumableCounts = new Dictionary<ConsumableType, int>
        {
            [ConsumableType.FuelTank] = 0,
            [ConsumableType.Grindstone] = 0,
            [ConsumableType.HealthPack] = 0,
            [ConsumableType.SmokingPipe] = 0,
            [ConsumableType.Dynamite] = 0,
            [ConsumableType.Antidote] = 0
        };

        consumableNames = new Dictionary<ConsumableType, string>
        {
            [ConsumableType.FuelTank] = "Топливо",
            [ConsumableType.Grindstone] = "Точильный камень",
            [ConsumableType.HealthPack] = "Еда",
            [ConsumableType.SmokingPipe] = "Трубка",
            [ConsumableType.Dynamite] = "Динамит",
            [ConsumableType.Antidote] = "Противоядие"
        };

        consumableRecoveries = new Dictionary<ConsumableType, float>
        {
            [ConsumableType.FuelTank] = fuelTankRecovery,
            [ConsumableType.Grindstone] = grindstoneRecovery,
            [ConsumableType.HealthPack] = healthPackRecovery,
            [ConsumableType.SmokingPipe] = smokingPipeRecovery,
        };
        UpdateHotbar();
    }

    private void Start()
    {
        if (!ServiceInfo.TutorialDone)
            loaded = true;
    }

    private void UpdateHotbar()
    {
        if (HotbarController.Instanse != null)
            HotbarController.Instanse.UpdateConsumablesCount(consumableCounts);
    }
}
