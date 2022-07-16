using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Consumables : MonoBehaviour, IStorage
{
    [Header("Counts")]
    [SerializeField] private int fuelTanksCount = 3;
    [SerializeField] private int grindstonesCount = 3;
    [SerializeField] private int healthPacksCount = 3;
    [SerializeField] private int smokingPipesCount = 3;
    [SerializeField] private int ropesCount = 3;

    [Header("Recoveries")]
    [SerializeField] private float fuelTankRecovery = 30f;
    [SerializeField] private float grindstoneRecovery = 30f;
    [SerializeField] private float healthPackRecovery = 30f;
    [SerializeField] private float smokingPipeRecovery = 30f;

    [Header("SFX")]
    [SerializeField] private SFX useConsumableSFX;

    private Dictionary<ConsumableType, int> consumableCounts;
    private Dictionary<ConsumableType, string> consumableNames;
    private Dictionary<ConsumableType, float> consumableRecoveries;
    private bool loaded = false;

    public void Save()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.FuelTanksCount, consumableCounts[ConsumableType.FuelTank]);
        PlayerPrefs.SetInt(PlayerPrefsKeys.GrindstonesCount, consumableCounts[ConsumableType.Grindstone]);
        PlayerPrefs.SetInt(PlayerPrefsKeys.HealthPacksCount, consumableCounts[ConsumableType.HealthPack]);
        PlayerPrefs.SetInt(PlayerPrefsKeys.SmokingPipesCount, consumableCounts[ConsumableType.SmokingPipe]);
        PlayerPrefs.SetInt(PlayerPrefsKeys.RopesCount, consumableCounts[ConsumableType.Rope]);
    }

    public void Load()
    {
        consumableCounts = new Dictionary<ConsumableType, int>
        {
            [ConsumableType.FuelTank] = PlayerPrefs.GetInt(PlayerPrefsKeys.FuelTanksCount, fuelTanksCount),
            [ConsumableType.Grindstone] = PlayerPrefs.GetInt(PlayerPrefsKeys.GrindstonesCount, grindstonesCount),
            [ConsumableType.HealthPack] = PlayerPrefs.GetInt(PlayerPrefsKeys.HealthPacksCount, healthPacksCount),
            [ConsumableType.SmokingPipe] = PlayerPrefs.GetInt(PlayerPrefsKeys.SmokingPipesCount, smokingPipesCount),
            [ConsumableType.Rope] = PlayerPrefs.GetInt(PlayerPrefsKeys.RopesCount, ropesCount),
            [ConsumableType.Antidote] = PlayerPrefs.GetInt(PlayerPrefsKeys.AntidotesCount, 0)
        };
        UpdateHotbar();
        loaded = true;
    }

    public void Add(ConsumableType type, int count)
    {
        if (count < 0)
        {
            useConsumableSFX.Play();
        }
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
            [ConsumableType.FuelTank] = 3,
            [ConsumableType.Grindstone] = 3,
            [ConsumableType.HealthPack] = 3,
            [ConsumableType.SmokingPipe] = 3,
            [ConsumableType.Rope] = 3,
            [ConsumableType.Antidote] = 3
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
            [ConsumableType.Rope] = 0,
            [ConsumableType.Antidote] = 0
        };

        consumableNames = new Dictionary<ConsumableType, string>
        {
            [ConsumableType.FuelTank] = "Топливо",
            [ConsumableType.Grindstone] = "Точильный камень",
            [ConsumableType.HealthPack] = "Еда",
            [ConsumableType.SmokingPipe] = "Трубка",
            [ConsumableType.Rope] = "Канат",
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
