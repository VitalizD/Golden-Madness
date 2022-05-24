using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour, IStorage
{
    [SerializeField] private int maxCapacity = 100;
    [SerializeField] private int currentFullness = 0;
    [SerializeField] private string fullInventoryPhrase = "Мой рюкзак заполнен!";
    [SerializeField] private string cannotTakePhrase = "Мне больше не унести!";

    private PlayerDialogWindow dialogWindow;

    private Dictionary<ResourceTypes, int> resourcesCounts;
    //private int defaultMaxCapacity;

    public int MaxCapacity { get => maxCapacity; set => maxCapacity = value; }

    public int CurrentFullness { get => currentFullness; }

    private void Awake()
    {
        dialogWindow = transform.GetChild(ServiceInfo.ChildIndexOfDialogWindow).GetComponent<PlayerDialogWindow>();
        //defaultMaxCapacity = maxCapacity;
        Clear();
    }

    public int GetOne(ResourceTypes resourse) => resourcesCounts[resourse];

    public Dictionary<ResourceTypes, int> GetAll() => resourcesCounts;

    public void Add(ResourceTypes resource)
    {
        if (currentFullness >= maxCapacity)
        {
            dialogWindow.gameObject.SetActive(true);
            dialogWindow.Show(cannotTakePhrase, 2f);

            return;
        }

        ++currentFullness;
        ++resourcesCounts[resource];

        if (currentFullness == maxCapacity)
        {
            dialogWindow.gameObject.SetActive(true);
            dialogWindow.Show(fullInventoryPhrase, 4f);
        }
    }

    public void Remove(ResourceTypes resource, int count)
    {
        resourcesCounts[resource] -= count;

        if (resourcesCounts[resource] < 0)
        {
            resourcesCounts[resource] = 0;
            RecalculateCapacity();
        }    
    }

    public void Clear()
    {
        currentFullness = 0;
        resourcesCounts = new Dictionary<ResourceTypes, int>
        {
            [ResourceTypes.Coal] = 0,
            [ResourceTypes.GoldOre] = 0,
            [ResourceTypes.IronOre] = 0,
            [ResourceTypes.Quartz] = 0
        };
    }

    public void Save()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.BackpackCapacity, maxCapacity);
    }

    public void Load()
    {
        maxCapacity = PlayerPrefs.GetInt(PlayerPrefsKeys.BackpackCapacity, maxCapacity);
    }

    private void RecalculateCapacity()
    {
        currentFullness = 0;
        foreach (var value in resourcesCounts.Values)
            currentFullness += value;
    }
}
