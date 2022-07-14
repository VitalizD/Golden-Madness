using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ResourcesController : MonoBehaviour
{
    public static ResourcesController Instanse { get; private set; } = null;

    private struct ResourceInfo
    {
        public string title;
        public Sprite sprite;
        public TextMeshProUGUI count;

        public ResourceInfo(string title, Sprite sprite, TextMeshProUGUI count)
        {
            this.title = title;
            this.sprite = sprite;
            this.count = count;
        }
    }

    [SerializeField] private bool inVillage = false;
    [SerializeField] private float timeBeforeHidingOneResourcePanel = 2f;

    [Space]

    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private Color maxFullnessColor;
    [SerializeField] private Transform oneResource;
    [SerializeField] private GameObject backpack;
    [SerializeField] private GameObject box;

    [Header("Panels")]
    [SerializeField] private GameObject normalPanel;
    [SerializeField] private GameObject allResourcesPanel;
    [SerializeField] private GameObject oneRecourcePanel;

    [Header("Recources Counts")]
    [SerializeField] private TextMeshProUGUI quartzCount;
    [SerializeField] private TextMeshProUGUI ironCount;
    [SerializeField] private TextMeshProUGUI goldCount;
    [SerializeField] private TextMeshProUGUI coalCount;

    [Header("SFX")]
    [SerializeField] private SFX menuPopUpSFX;

    private Image oneResourceIcon;
    private TextMeshProUGUI oneResourceTitle;
    private TextMeshProUGUI oneResourceCount;

    private Backpack playerBackpack;

    private Dictionary<ResourceType, ResourceInfo> resourcesInfo;
    private bool allResourcesShowed = false;
    private Color capacityTextNormalColor;

    private Coroutine hideOneResourcePanel;

    public void ShowOneResource(ResourceType type, int count = 1)
    {
        if (type == ResourceType.None)
            return;

        ActiveOneResourcePanel();

        var resource = resourcesInfo[type];
        oneResourceTitle.text = resource.title;
        oneResourceCount.text = $"{playerBackpack.GetOne(type)} <color=green>+ {count}</color>";

        if (resource.sprite != null)
            oneResourceIcon.sprite = resource.sprite;

        hideOneResourcePanel = StartCoroutine(HideOneResourcePanel());
    }

    public void SetCapacity(int value, int maxValue)
    {
        if (value >= maxValue)
            capacityText.color = maxFullnessColor;
        else
            capacityText.color = capacityTextNormalColor;

        capacityText.text = $"{value} / {maxValue}";
    }

    public void UpdateResourcesCounts()
    {
        var resources = new Dictionary<ResourceType, int>();

        if (inVillage)
            resources = VillageController.instanse.GetAllRecources();
        else if (playerBackpack != null)
            resources = playerBackpack.GetAll();

        if (resources.Count == 0)
            return;

        foreach (var type in resourcesInfo.Keys)
            resourcesInfo[type].count.text = resources[type].ToString();
    }

    private void Awake()
    {
        if (Instanse == null)
            Instanse = this;
        else if (Instanse == this)
            Destroy(gameObject);

        oneResourceIcon = oneResource.GetChild(0).GetComponent<Image>();
        oneResourceTitle = oneResource.GetChild(1).GetComponent<TextMeshProUGUI>();
        oneResourceCount = oneResource.GetChild(2).GetComponent<TextMeshProUGUI>();

        capacityTextNormalColor = capacityText.color;

        allResourcesPanel.SetActive(false);
        oneRecourcePanel.SetActive(false);
    }

    private void Start()
    {
        playerBackpack = Player.Instanse.GetComponent<Backpack>();

        var icons = SpritesStorage.Instanse;
        resourcesInfo = new Dictionary<ResourceType, ResourceInfo>
        {
            [ResourceType.Quartz] = new ResourceInfo("Êגאנצ", icons.GetResource(ResourceType.Quartz), quartzCount),
            [ResourceType.IronOre] = new ResourceInfo("Æוכוחמ", icons.GetResource(ResourceType.IronOre), ironCount),
            [ResourceType.GoldOre] = new ResourceInfo("Çמכמעמ", icons.GetResource(ResourceType.GoldOre), goldCount),
            [ResourceType.Coal] = new ResourceInfo("Óדמכü", icons.GetResource(ResourceType.Coal), coalCount)
        };

        if (inVillage)
        {
            backpack.SetActive(false);
            box.SetActive(true);
            ShowAllResources();
        }
    }

    private void Update()
    {
        if (!inVillage)
        {
            if (Input.GetKey(KeyCode.Tab) && !allResourcesShowed)
            {
                menuPopUpSFX.Play();
                allResourcesShowed = true;
                ShowAllResources();
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                allResourcesShowed = false;
                HideAllResources();
            }
        }
    }

    private void ShowAllResources()
    {
        ActiveAllResourcesPanel();
        UpdateResourcesCounts();
    }

    private void HideAllResources()
    {
        ActiveNormalPanel();
    }

    private IEnumerator HideOneResourcePanel()
    {
        yield return new WaitForSeconds(timeBeforeHidingOneResourcePanel);
        ActiveNormalPanel();
    }

    private void ActiveAllResourcesPanel()
    {
        if (hideOneResourcePanel != null) StopCoroutine(hideOneResourcePanel);
        normalPanel.SetActive(false);
        oneRecourcePanel.SetActive(false);
        allResourcesPanel.SetActive(true);
    }

    private void ActiveNormalPanel()
    {
        if (hideOneResourcePanel != null) StopCoroutine(hideOneResourcePanel);
        normalPanel.SetActive(true);
        oneRecourcePanel.SetActive(false);
        allResourcesPanel.SetActive(false);
    }

    private void ActiveOneResourcePanel()
    {
        if (hideOneResourcePanel != null) StopCoroutine(hideOneResourcePanel);
        normalPanel.SetActive(false);
        oneRecourcePanel.SetActive(true);
        allResourcesPanel.SetActive(false);
    }
}
