using UnityEngine;
using System.Collections.Generic;

public class VillageController : MonoBehaviour, IStorage
{
    public static VillageController instanse = null;

    [SerializeField] private bool loadResources = true;
    [SerializeField] private GameObject triggers;

    [Header("Resources Sprites")]
    [SerializeField] private Sprite goldIcon;
    [SerializeField] private Sprite coalIcon;
    [SerializeField] private Sprite quartzIcon;
    [SerializeField] private Sprite ironIcon;

    private Dictionary<ResourceTypes, int> resourcesCounts = new Dictionary<ResourceTypes, int>();
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

    public void FinishTutorial()
    {
        ServiceInfo.TutorialDone = true;
        PlayerPrefs.SetString(PlayerPrefsKeys.TutorialDone, true.ToString());
    }

    public void Save()
    {
        ResourcesSaver.Save(resourcesCounts);
    }

    public void Load()
    {
        var defaultValue = 0;
        resourcesCounts = new Dictionary<ResourceTypes, int>
        {
            [ResourceTypes.Coal] = PlayerPrefs.GetInt(PlayerPrefsKeys.CoalCount, defaultValue),
            [ResourceTypes.GoldOre] = PlayerPrefs.GetInt(PlayerPrefsKeys.GoldCount, defaultValue),
            [ResourceTypes.IronOre] = PlayerPrefs.GetInt(PlayerPrefsKeys.IronCount, defaultValue),
            [ResourceTypes.Quartz] = PlayerPrefs.GetInt(PlayerPrefsKeys.QuartzCount, defaultValue)
        };
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
        if (loadResources)
            Load();

        SetPlayerParameters();

        if (!ServiceInfo.TutorialDone)
            triggers.SetActive(true);
    }

    private void SetPlayerParameters()
    {
        var player = Player.instanse;
        player.Load();
        player.Health = 100;
        player.PickaxeStrength = 100f;
        player.GetComponent<SanityController>().Sanity = 100f;
        player.transform.GetChild(ServiceInfo.ChildIndexOfLamp).GetComponent<Lamp>().FuelCount = 100f;
    }
}
