using UnityEngine;
using System.Collections.Generic;

public class VillageController : MonoBehaviour, IStorage
{
    public static VillageController instanse = null;

    [SerializeField] private bool loadResources = true;
    [SerializeField] private GameObject triggers;

    private Dictionary<ResourceTypes, int> resourcesCounts = new Dictionary<ResourceTypes, int>();
    private Dictionary<ResourceTypes, Sprite> resourcesSprites = new Dictionary<ResourceTypes, Sprite>();

    public int GetResourcesCount(ResourceTypes type)
    {
        if (resourcesCounts.ContainsKey(type))
            return resourcesCounts[type];
        return 0;
    }

    public Dictionary<ResourceTypes, int> GetAllRecources() => resourcesCounts;

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

    public void FinishTutorial()
    {
        ServiceInfo.TutorialDone = true;
        PlayerPrefs.SetString(PlayerPrefsKeys.TutorialDone, true.ToString());
    }

    public void Save()
    {
        ResourcesSaver.ReplaceInVillage(resourcesCounts);
    }

    public void Load()
    {
        var defaultValue = 0;
        resourcesCounts = new Dictionary<ResourceTypes, int>
        {
            [ResourceTypes.Coal] = PlayerPrefs.GetInt(ResourceTypes.Coal.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue),
            [ResourceTypes.GoldOre] = PlayerPrefs.GetInt(ResourceTypes.GoldOre.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue),
            [ResourceTypes.IronOre] = PlayerPrefs.GetInt(ResourceTypes.IronOre.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue),
            [ResourceTypes.Quartz] = PlayerPrefs.GetInt(ResourceTypes.Quartz.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue)
        };
    }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
            Destroy(gameObject);

        var icons = SpritesStorage.instanse;
        resourcesSprites = new Dictionary<ResourceTypes, Sprite>
        {
            [ResourceTypes.GoldOre] = icons.GoldIcon,
            [ResourceTypes.Coal] = icons.CoalIcon,
            [ResourceTypes.IronOre] = icons.IronIcon,
            [ResourceTypes.Quartz] = icons.QuartzIcon
        };
    }

    private void Start()
    {
        if (loadResources)
        {
            Load();
            if (!ServiceInfo.TutorialDone)
            {
                resourcesCounts[ResourceTypes.GoldOre] = 6;
                triggers.SetActive(true);
            }
        }

        SetPlayerParameters();
    }

    private void SetPlayerParameters()
    {
        var player = Player.instanse;
        player.Load();
        player.Health = 100;
        player.PickaxeStrength = 100f;
        player.GetComponent<SanityController>().Sanity = 100f;
        player.transform.GetChild(ServiceInfo.ChildIndexOfLamp).GetComponent<Lamp>().FuelCount = 100f;
        player.GetComponent<Consumables>().SetDefaultValues();
    }
}
