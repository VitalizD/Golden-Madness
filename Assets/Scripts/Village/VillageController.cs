using UnityEngine;
using System.Collections.Generic;

public class VillageController : MonoBehaviour
{
    private Dictionary<ResourceTypes, int> resources;

    public int GetResourcesCount(ResourceTypes type) => resources[type];

    private void Start()
    {
        resources = DataStorage.Resources;
    }
}
