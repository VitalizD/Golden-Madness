using System.Collections;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField] private bool enableFuelDecrease = false;
    [SerializeField] [Range(0, 100)] private float fuelCount = 100f;
    [SerializeField] private float minLightRange = 8f;
    [SerializeField] private float maxLightRange = 25f;
    [SerializeField] private float fuelDecreaseValue = 0.5f;
    [SerializeField] private float timeFuelDecrease = 1f;

    private Light light_;
    private Consumables consumables;

    private Coroutine decreaseFuel;

    private float FuelCount
    {
        get => fuelCount;
        set
        {
            if (value < 0)
                fuelCount = 0;
            else if (value > 100)
                fuelCount = 100;
            else
                fuelCount = value;
            light_.range = Mathf.Lerp(minLightRange, maxLightRange, fuelCount / 100);
        }
    }

    private void Awake()
    {
        light_ = GetComponent<Light>();
        consumables = transform.parent.GetComponent<Consumables>();
    }

    private void Start()
    {
        FuelCount = fuelCount;
        decreaseFuel = StartCoroutine(DecreaseLampFuel());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && consumables.FuelTanksCount > 0)
        {
            FuelCount += consumables.FuelTankRecovery;
            --consumables.FuelTanksCount;
        }
    }

    private IEnumerator DecreaseLampFuel()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeFuelDecrease);
            if (enableFuelDecrease)
                FuelCount -= fuelDecreaseValue;
        }
    }
}
