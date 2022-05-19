using UnityEngine;
using System.Collections.Generic;

public class VillageController : MonoBehaviour
{
    public static VillageController instanse = null;

    [SerializeField] private bool loadFromStorage = true;

    [Header("Resources Sprites")]
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite coalIcon;
    [SerializeField] private Sprite quartzIcon;
    [SerializeField] private Sprite ironIcon;

    private Dictionary<ResourceTypes, int> resourcesCounts = new Dictionary<ResourceTypes, int>
    {
        [ResourceTypes.GoldOre] = 6
    };
    private Dictionary<ResourceTypes, Sprite> resourcesSprites = new Dictionary<ResourceTypes, Sprite>();

    public int GetResourcesCount(ResourceTypes type)
    {
        if (resourcesCounts.ContainsKey(type))
            return resourcesCounts[type];
        return 0;
    }

    public Sprite GetResourceSprite(ResourceTypes type)
    {
        if (resourcesSprites.ContainsKey(type))
            return resourcesSprites[type];
        return null;
    }

    public void AddResource(ResourceTypes type, int count)
    {
        resourcesCounts[type] += count;
        if (resourcesCounts[type] < 0)
            resourcesCounts[type] = 0;
    }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
            Destroy(gameObject);

        resourcesSprites = new Dictionary<ResourceTypes, Sprite>
        {
            [ResourceTypes.GoldOre] = goldIcon,
            [ResourceTypes.Coal] = coalIcon,
            [ResourceTypes.IronOre] = ironIcon,
            [ResourceTypes.Quartz] = quartzIcon
        };
    }

    private void Start()
    {
        if (loadFromStorage)
            resourcesCounts = DataStorage.Resources;
    }
}
