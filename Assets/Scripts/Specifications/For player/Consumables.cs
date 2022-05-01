using UnityEngine;

public class Consumables : MonoBehaviour
{
    [SerializeField] private int maxCount = 5;

    [Header("Fuel tanks")]
    [SerializeField] private int fuelTanksCount = 1;
    [SerializeField] [Range(0, 100)] private float fuelTankRecovery = 50f;

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

    public float FuelTankRecovery { get => fuelTankRecovery; }
}
