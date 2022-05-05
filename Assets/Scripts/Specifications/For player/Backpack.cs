using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    [SerializeField] private int maxCapacity = 100;
    [SerializeField] private int currentCapacity = 0;
    [SerializeField] private string fullInventoryPhrase = "Мой рюкзак заполнен!";
    [SerializeField] private string cannotTakePhrase = "Мне больше не унести!";

    private PlayerDialogWindow dialogWindow;

    private readonly Dictionary<ResourceTypes, int> resourcesCounts = new Dictionary<ResourceTypes, int>
    {
        [ResourceTypes.Coal] = 0,
        [ResourceTypes.GoldOre] = 0,
        [ResourceTypes.IronOre] = 0,
        [ResourceTypes.Quartz] = 0
    };

    private readonly int childIndexOfDialogWindow = 2;

    private void Awake()
    {
        dialogWindow = transform.GetChild(childIndexOfDialogWindow).GetComponent<PlayerDialogWindow>();
    }

    public int GetOne(ResourceTypes resourse) => resourcesCounts[resourse];

    public Dictionary<ResourceTypes, int> GetAll() => resourcesCounts;

    public void Add(ResourceTypes resource)
    {
        if (currentCapacity >= maxCapacity)
        {
            dialogWindow.gameObject.SetActive(true);
            dialogWindow.Show(cannotTakePhrase, 2f);

            return;
        }

        ++currentCapacity;
        ++resourcesCounts[resource];

        if (currentCapacity == maxCapacity)
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
        currentCapacity = 0;
        foreach (var key in resourcesCounts.Keys)
            resourcesCounts[key] = 0;
    }

    private void RecalculateCapacity()
    {
        currentCapacity = 0;
        foreach (var value in resourcesCounts.Values)
            currentCapacity += value;
    }
}
