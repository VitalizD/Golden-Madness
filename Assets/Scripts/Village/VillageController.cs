using UnityEngine;
using System.Collections.Generic;

public class VillageController : MonoBehaviour, IStorage
{
    public static VillageController instanse = null;

    [SerializeField] private bool loadResources = true;
    [SerializeField] private GameObject triggers;

    private Dictionary<ResourceType, int> resourcesCounts = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, Sprite> resourcesSprites = new Dictionary<ResourceType, Sprite>();

    public int GetResourcesCount(ResourceType type)
    {
        if (resourcesCounts.ContainsKey(type))
            return resourcesCounts[type];
        return 0;
    }

    public Dictionary<ResourceType, int> GetAllRecources() => resourcesCounts;

    public Sprite GetResourceSprite(ResourceType type)
    {
        if (resourcesSprites.ContainsKey(type))
            return resourcesSprites[type];
        return null;
    }

    public void AddResource(ResourceType type, int count)
    {
        resourcesCounts[type] += count;
        if (resourcesCounts[type] < 0)
            resourcesCounts[type] = 0;

        ResourcesController.Instanse.UpdateResourcesCounts();
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
        resourcesCounts = new Dictionary<ResourceType, int>
        {
            [ResourceType.Coal] = PlayerPrefs.GetInt(ResourceType.Coal.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue),
            [ResourceType.GoldOre] = PlayerPrefs.GetInt(ResourceType.GoldOre.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue),
            [ResourceType.IronOre] = PlayerPrefs.GetInt(ResourceType.IronOre.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue),
            [ResourceType.Quartz] = PlayerPrefs.GetInt(ResourceType.Quartz.ToString() + PlayerPrefsKeys.ResourcesCountPrefix, defaultValue)
        };
    }

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
            Destroy(gameObject);
    }

    private void Start()
    {
        var icons = SpritesStorage.Instanse;
        resourcesSprites = new Dictionary<ResourceType, Sprite>
        {
            [ResourceType.GoldOre] = icons.GetResource(ResourceType.GoldOre),
            [ResourceType.Coal] = icons.GetResource(ResourceType.Coal),
            [ResourceType.IronOre] = icons.GetResource(ResourceType.IronOre),
            [ResourceType.Quartz] = icons.GetResource(ResourceType.Quartz)
        };

        if (loadResources)
        {
            Load();
            if (!ServiceInfo.TutorialDone)
            {
                resourcesCounts[ResourceType.GoldOre] = 6;
                PlayerPrefs.SetString(PlayerPrefsKeys.TutorialDoneInCave, "true");
                triggers.SetActive(true);
            }
        }
        //resourcesCounts[ResourceType.GoldOre] = 20;
        //PlayerPrefs.DeleteAll();

        PlayerPrefs.SetInt(PlayerPrefsKeys.CurrentLevelNumber, 0);
        SetPlayerParameters();
    }

    private void SetPlayerParameters()
    {
        var player = Player.Instanse;
        player.Load();
        player.Health = 100;
        player.PickaxeStrength = 100f;
        player.SetFuelCount(100f);
        player.GetComponent<SanityController>().Sanity = 100f;
        player.GetComponent<Consumables>().SetDefaultValues();
    }
}
