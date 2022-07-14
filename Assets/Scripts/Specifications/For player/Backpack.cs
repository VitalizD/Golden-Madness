using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour, IStorage
{
    [SerializeField] private int maxCapacity = 100;
    [SerializeField] private int currentFullness = 0;
    [SerializeField] private float fullInventorySpeedMultiplier = 0.85f;
    [SerializeField] private string fullInventoryPhrase = "Мой рюкзак заполнен!";
    [SerializeField] private string cannotTakePhrase = "Мне больше не унести!";

    private Dictionary<ResourceType, int> resourcesCounts;
    private static Dictionary<ResourceType, string> resourcesNames = new Dictionary<ResourceType, string>
    {
        [ResourceType.Coal] = "Уголь",
        [ResourceType.GoldOre] = "Золото",
        [ResourceType.IronOre] = "Железо",
        [ResourceType.Quartz] = "Кристалл кварца"
    };

    public int MaxCapacity 
    { 
        get => maxCapacity; 
        set
        {
            maxCapacity = value;
            UpdateTextFullness();
        }
    }

    public static string GetResourceName(ResourceType type) => resourcesNames[type];

    public bool IsFull() => currentFullness >= maxCapacity;

    public int CurrentFullness { get => currentFullness; }

    public float FullInventorySpeedMultiplier { get => fullInventorySpeedMultiplier; }

    private void Start()
    {
        Clear();
    }

    public int GetOne(ResourceType resourse) => resourcesCounts[resourse];

    public Dictionary<ResourceType, int> GetAll() => resourcesCounts;

    public void Add(ResourceType resource, int count = 1)
    {
        if (currentFullness >= maxCapacity)
        {
            Player.Instanse.Say(cannotTakePhrase, 2f);
            return;
        }

        currentFullness += count;
        resourcesCounts[resource] += count;

        if (IsFull()) Player.Instanse.Speed = Player.Instanse.DefaultSpeed * FullInventorySpeedMultiplier;

        if (currentFullness == maxCapacity)
            Player.Instanse.Say(fullInventoryPhrase, 4f);

        if (currentFullness <= maxCapacity)
            ResourcesController.Instanse.ShowOneResource(resource, count);

        UpdateTextFullness();
    }

    public void Remove(ResourceType resource, int count)
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
        resourcesCounts = new Dictionary<ResourceType, int>
        {
            [ResourceType.Coal] = 0,
            [ResourceType.GoldOre] = 0,
            [ResourceType.IronOre] = 0,
            [ResourceType.Quartz] = 0
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
        var newResourcesCounts = new Dictionary<ResourceType, int>();
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
