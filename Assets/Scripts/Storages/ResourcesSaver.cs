using System.Collections.Generic;
using UnityEngine;

public static class ResourcesSaver
{
    public static void SaveInVillage(Dictionary<ResourceTypes, int> resources)
    {
        foreach (var type in resources.Keys)
            PlayerPrefs.SetInt(type.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, resources[type]);
    }

    public static void SaveInBackpack(Dictionary<ResourceTypes, int> resources)
    {
        foreach (var type in resources.Keys)
            PlayerPrefs.SetInt(type.ToString() + PlayerPrefsKeys.ResourcesCountInBackpackPrefix, resources[type]);
    }
}
