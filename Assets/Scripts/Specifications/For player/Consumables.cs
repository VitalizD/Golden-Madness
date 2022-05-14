using UnityEngine;
using System.Collections.Generic;

public class Consumables : MonoBehaviour
{
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

}
