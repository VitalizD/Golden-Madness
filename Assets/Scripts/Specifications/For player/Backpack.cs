using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour, IStorage
{
    [SerializeField] private int maxCapacity = 100;
    [SerializeField] private int currentFullness = 0;
    [SerializeField] private float fullInventorySpeedMultiplier = 0.85f;
    [SerializeField] private string fullInventoryPhrase = "Мой рюкзак заполнен!";
    [SerializeField] private string cannotTakePhrase = "Мне больше не унести!";

    private Dictionary<ResourceTypes, int> resourcesCounts;

    public int MaxCapacity 
    { 
        get => maxCapacity; 
        set
        {
            maxCapacity = value;
            UpdateTextFullness();
        }
    }

    public bool IsFull() => currentFullness == maxCapacity;

    public int CurrentFullness { get => currentFullness; }

    public float FullInventorySpeedMultiplier { get => fullInventorySpeedMultiplier; }

    private void Start()
    {
        Clear();
    }

    public int GetOne(ResourceTypes resourse) => resourcesCounts[resourse];

    public Dictionary<ResourceTypes, int> GetAll() => resourcesCounts;

    public void Add(ResourceTypes resource)
    {
        if (currentFullness >= maxCapacity)
        {
            Player.Instanse.Say(cannotTakePhrase, 2f);
            return;
        }

        ++currentFullness;
        ++resourcesCounts[resource];

        if (IsFull()) Player.Instanse.Speed = Player.Instanse.DefaultSpeed * FullInventorySpeedMultiplier;

        if (currentFullness == maxCapacity)
            Player.Instanse.Say(fullInventoryPhrase, 4f);

        if (currentFullness <= maxCapacity)
            ResourcesController.Instanse.ShowOneResource(resource);

        UpdateTextFullness();
    }

    public void Remove(ResourceTypes resource, int count)
    {
        resourcesCounts[resource] -= count;

        if (resourcesCounts[resource] < 0)
        {
            resourcesCounts[resource] = 0;
            RecalculateFullness();
        }
        UpdateTextFullness();
    }

    public void Clear()
    {
        Player.Instanse.Speed = Player.Instanse.DefaultSpeed;

        currentFullness = 0;
        resourcesCounts = new Dictionary<ResourceTypes, int>
        {
            [ResourceTypes.Coal] = 0,
            [ResourceTypes.GoldOre] = 0,
            [ResourceTypes.IronOre] = 0,
            [ResourceTypes.Quartz] = 0
        };

        if (ResourcesController.Instanse != null)
            ResourcesController.Instanse.UpdateResourcesCounts();

        UpdateTextFullness();
    }

    public void Save()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.BackpackCapacity, maxCapacity);
        ResourcesSaver.SaveInBackpack(resourcesCounts);
    }

    public void Load()
    {
        maxCapacity = PlayerPrefs.GetInt(PlayerPrefsKeys.BackpackCapacity, maxCapacity);
        var newResourcesCounts = new Dictionary<ResourceTypes, int>();
        foreach (var type in resourcesCounts.Keys)
        {
            var key = type.ToString() + PlayerPrefsKeys.ResourcesCountInBackpackPrefix;
            newResourcesCounts[type] = PlayerPrefs.GetInt(key, 0);
            PlayerPrefs.DeleteKey(key);
        }
        resourcesCounts = newResourcesCounts;
        RecalculateFullness();
    }

    private void RecalculateFullness()
    {
        currentFullness = 0;
        foreach (var value in resourcesCounts.Values)
            currentFullness += value;
        UpdateTextFullness();
    }

    private void UpdateTextFullness()
    {
        if (ResourcesController.Instanse != null)
            ResourcesController.Instanse.SetCapacity(currentFullness, maxCapacity);
    }
}
