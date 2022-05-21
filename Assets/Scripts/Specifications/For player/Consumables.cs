using UnityEngine;
using System.Collections.Generic;

public class Consumables : MonoBehaviour, IStorage
{
    //private readonly struct DefaultValues
    //{
    //    public readonly int fuelTanksCount;
    //    public readonly int grindstonesCount;
    //    public readonly int healthPacksCount;
    //    public readonly int smokingPipesCount;

    //    public DefaultValues(int fuelTanksCount, int grindstonesCount,
    //        int healthPacksCount, int smokingPipesCount)
    //    {
    //        this.fuelTanksCount = fuelTanksCount;
    //        this.grindstonesCount = grindstonesCount;
    //        this.healthPacksCount = healthPacksCount;
    //        this.smokingPipesCount = smokingPipesCount;
    //    }
    //}

    [SerializeField] private int maxCount = 5;

    [Header("Fuel tanks")]
    [SerializeField] private int fuelTanksCount = 1;
    [SerializeField] [Range(0, 100)] private static float fuelTankRecovery = 50f;

    [Header("Grindstones")]
    [SerializeField] private int grindstonesCount = 1;
    [SerializeField] [Range(0, 100)] private static float grindstoneRecovery = 50;

    [Header("Health packs")]
    [SerializeField] private int healthPacksCount = 1;
    [SerializeField] [Range(0, 100)] private static int healthPackRecovery = 50;
    
    [Header("Smoking pipes")]
    [SerializeField] private int smokingPipesCount = 1;
    [SerializeField] [Range(0, 100)] private static float smokingPipeRecovery = 50f;

    //private DefaultValues defaultValues;

    #region FuelTanks

    public int FuelTanksCount 
    { 
        get => fuelTanksCount;
        set
        {
            if (value < 0) fuelTanksCount = 0;
            else if (value > maxCount) fuelTanksCount = maxCount;
            else fuelTanksCount = value;
        }
    }

    public static float FuelTankRecovery { get => fuelTankRecovery; }

    #endregion

    #region HealthPacks

    public int HealthPacksCount { get => healthPacksCount;
        set 
        {
            if (value < 0) healthPacksCount = 0;
            else if (value > maxCount) healthPacksCount = maxCount;
            else healthPacksCount = value;
        } 
    }
    public static int HealthPacksRecovery { get => healthPackRecovery;}

    #endregion HealthPacks

    #region Smoking Pipes
    public int SmokingPipesCount { get => smokingPipesCount; 
        set 
        {
            if (value < 0) smokingPipesCount = 0;
            else if (value > maxCount) smokingPipesCount = maxCount;
            else smokingPipesCount = value;
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
            if (value < 0) grindstonesCount = 0;
            else if (value > maxCount) grindstonesCount = maxCount;
            else grindstonesCount = value;
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
        fuelTanksCount = PlayerPrefs.GetInt(PlayerPrefsKeys.FuelTanksCount, fuelTanksCount);
        grindstonesCount = PlayerPrefs.GetInt(PlayerPrefsKeys.GrindstonesCount, grindstonesCount);
        healthPacksCount = PlayerPrefs.GetInt(PlayerPrefsKeys.HealthPacksCount, healthPacksCount);
        smokingPipesCount = PlayerPrefs.GetInt(PlayerPrefsKeys.SmokingPipesCount, smokingPipesCount);
    }

    private void Start()
    {
        //defaultValues = new DefaultValues(fuelTanksCount, grindstonesCount, healthPacksCount, smokingPipesCount);
    }
}
