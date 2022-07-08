using System.Collections.Generic;
using UnityEngine;

public static class ResourcesSaver
{
    public static void ReplaceInVillage(Dictionary<ResourceType, int> resources)
    {
        foreach (var type in resources.Keys)
            PlayerPrefs.SetInt(type.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, resources[type]);
    }

    public static void AddInVillage(Dictionary<ResourceType, int> resources)
    {
        foreach (var type in resources.Keys)
        {
            var previousCount = PlayerPrefs.GetInt(type.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, 0);
            PlayerPrefs.SetInt(type.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, previousCount + resources[type]);
        }
    }

    public static void SaveInBackpack(Dictionary<ResourceType, int> resources)
    {
        foreach (var type in resources.Keys)
            PlayerPrefs.SetInt(type.ToString() + PlayerPrefsKeys.ResourcesCountInBackpackPrefix, resources[type]);
    }
}
