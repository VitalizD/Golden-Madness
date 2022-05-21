using System.Collections.Generic;
using UnityEngine;

public static class ResourcesSaver
{
    public static void Save(Dictionary<ResourceTypes, int> resources)
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.GoldCount, resources[ResourceTypes.GoldOre]);
        PlayerPrefs.SetInt(PlayerPrefsKeys.CoalCount, resources[ResourceTypes.Coal]);
        PlayerPrefs.SetInt(PlayerPrefsKeys.QuartzCount, resources[ResourceTypes.Quartz]);
        PlayerPrefs.SetInt(PlayerPrefsKeys.IronCount, resources[ResourceTypes.IronOre]);
    }
}
