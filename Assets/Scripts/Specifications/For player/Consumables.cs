using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Consumables : MonoBehaviour, IStorage
{
    [SerializeField] private static int maxCount = 5;

    [Header("Fuel tanks")]
    [SerializeField] private int fuelTanksCount = 1;
    private readonly static float fuelTankRecovery = 50f;

    [Header("Grindstones")]
    [SerializeField] private int grindstonesCount = 1;
    private readonly static float grindstoneRecovery = 50;

    [Header("Health packs")]
    [SerializeField] private int healthPacksCount = 1;
    private readonly static int healthPackRecovery = 50;

    [Header("Smoking pipes")]
    [SerializeField] private int smokingPipesCount = 1;
    private readonly static float smokingPipeRecovery = 50f;

    private bool loaded = false;

    public static int MaxCount { get => maxCount; set => maxCount = value; }

    #region FuelTanks

    public int FuelTanksCount 
    { 
        get => fuelTanksCount;
        set
        {
            var initialValue = fuelTanksCount;

            if (value < 0) fuelTanksCount = 0;
            else if (value > maxCount) fuelTanksCount = maxCount;
            else fuelTanksCount = value;

            if (HotbarController.Instanse != null)
                HotbarController.Instanse.SetConsumableCount(ConsumableType.FuelTank, fuelTanksCount);

            if (fuelTanksCount - initialValue > 0 && loaded)
                TakingConsumables.Instanse.AddConsumable("Топливо", fuelTanksCount - initialValue, SpritesStorage.instanse.FuelTank);
        }
    }

    public static float FuelTankRecovery { get => fuelTankRecovery; }

    #endregion

    #region HealthPacks

    public int HealthPacksCount 
    { 
        get => healthPacksCount;
        set 
        {
            var initialValue = healthPacksCount;

            if (value < 0) healthPacksCount = 0;
            else if (value > maxCount) healthPacksCount = maxCount;
            else healthPacksCount = value;

            if (HotbarController.Instanse != null)
                HotbarController.Instanse.SetConsumableCount(ConsumableType.HealthPack, healthPacksCount);

            if (healthPacksCount - initialValue > 0 && loaded)
                TakingConsumables.Instanse.AddConsumable("Еда", healthPacksCount - initialValue, SpritesStorage.instanse.HealthPack);
        } 
    }
    public static int HealthPacksRecovery { get => healthPackRecovery;}

    #endregion HealthPacks

    #region Smoking Pipes
    public int SmokingPipesCount { get => smokingPipesCount; 
        set 
        {
            var initialValue = smokingPipesCount;

            if (value < 0) smokingPipesCount = 0;
            else if (value > maxCount) smokingPipesCount = maxCount;
            else smokingPipesCount = value;

            if (HotbarController.Instanse != null)
                HotbarController.Instanse.SetConsumableCount(ConsumableType.SmokingPipe, smokingPipesCount);

            if (smokingPipesCount - initialValue > 0 && loaded)
                TakingConsumables.Instanse.AddConsumable("Трубка", smokingPipesCount - initialValue, SpritesStorage.instanse.SmokingPipe);
        }
    }
    public static float SmokingPipesRecovery { get => smokingPipeRecovery;}
    #endregion

    #region Grindstone
    public int GrindstonesCount
    {
        get => grindstonesCount;
        set
        {
            var initialValue = grindstonesCount;

            if (value < 0) grindstonesCount = 0;
            else if (value > maxCount) grindstonesCount = maxCount;
            else grindstonesCount = value;

            if (HotbarController.Instanse != null)
                HotbarController.Instanse.SetConsumableCount(ConsumableType.Grindstone, grindstonesCount);

            if (grindstonesCount - initialValue > 0 && loaded)
                TakingConsumables.Instanse.AddConsumable("Точильный камень", grindstonesCount - initialValue, SpritesStorage.instanse.Grindstone);
        }
    }
    public static float GrindstoneRecovery { get => grindstoneRecovery; }
    #endregion

    public void Save()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.FuelTanksCount, fuelTanksCount);
        PlayerPrefs.SetInt(PlayerPrefsKeys.GrindstonesCount, grindstonesCount);
        PlayerPrefs.SetInt(PlayerPrefsKeys.HealthPacksCount, healthPacksCount);
        PlayerPrefs.SetInt(PlayerPrefsKeys.SmokingPipesCount, smokingPipesCount);
    }

    public void Load()
    {
        FuelTanksCount = PlayerPrefs.GetInt(PlayerPrefsKeys.FuelTanksCount, fuelTanksCount);
        GrindstonesCount = PlayerPrefs.GetInt(PlayerPrefsKeys.GrindstonesCount, grindstonesCount);
        HealthPacksCount = PlayerPrefs.GetInt(PlayerPrefsKeys.HealthPacksCount, healthPacksCount);
        SmokingPipesCount = PlayerPrefs.GetInt(PlayerPrefsKeys.SmokingPipesCount, smokingPipesCount);
        loaded = true;
    }

    public void SetDefaultValues()
    {
        FuelTanksCount = 1;
        HealthPacksCount = 1;
        SmokingPipesCount = 1;
        GrindstonesCount = 1;
    }

    private void Start()
    {
        if (!ServiceInfo.TutorialDone)
            loaded = true;
    }
}
