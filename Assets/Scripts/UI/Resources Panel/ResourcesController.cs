using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class ResourcesController : MonoBehaviour
{
    public static ResourcesController instanse = null;

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

    [SerializeField] private float timeBeforeHidingOneResourcePanel = 2f;

    [Space]

    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private Color maxFullnessColor;
    [SerializeField] private Transform oneResource;

    [Header("Panels")]
    [SerializeField] private GameObject normalPanel;
    [SerializeField] private GameObject allResourcesPanel;
    [SerializeField] private GameObject oneRecourcePanel;

    [Header("Recources Counts")]
    [SerializeField] private TextMeshProUGUI quartzCount;
    [SerializeField] private TextMeshProUGUI ironCount;
    [SerializeField] private TextMeshProUGUI goldCount;
    [SerializeField] private TextMeshProUGUI coalCount;

    private Image oneResourceIcon;
    private TextMeshProUGUI oneResourceTitle;
    private TextMeshProUGUI oneResourceCount;

    private Backpack backpack;

    private Dictionary<ResourceTypes, ResourceInfo> resourcesInfo;
    private bool allResourcesShowed = false;
    private Color capacityTextNormalColor;

    private Coroutine hideOneResourcePanel;

    public void ShowOneResource(ResourceTypes type)
    {
        if (type == ResourceTypes.None)
            return;

        ActiveOneResourcePanel();

        var resource = resourcesInfo[type];
        oneResourceTitle.text = resource.title;
        oneResourceCount.text = $"{backpack.GetOne(type)} <color=green>+ 1</color>";

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

    private void Awake()
    {
        if (instanse == null)
            instanse = this;
        else if (instanse == this)
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
        backpack = Player.instanse.GetComponent<Backpack>();

        var icons = SpritesStorage.instanse;
        resourcesInfo = new Dictionary<ResourceTypes, ResourceInfo>
        {
            [ResourceTypes.Quartz] = new ResourceInfo("Êגאנצ", icons.QuartzIcon, quartzCount),
            [ResourceTypes.IronOre] = new ResourceInfo("Æוכוחמ", icons.IronIcon, ironCount),
            [ResourceTypes.GoldOre] = new ResourceInfo("Çמכמעמ", icons.GoldIcon, goldCount),
            [ResourceTypes.Coal] = new ResourceInfo("Óדמכü", icons.CoalIcon, coalCount)
        };
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.I) && !allResourcesShowed)
        {
            allResourcesShowed = true;
            ShowAllResources();
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            allResourcesShowed = false;
            HideAllResources();
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

    private void UpdateResourcesCounts()
    {
        var resources = backpack.GetAll();
        foreach (var type in resourcesInfo.Keys)
            resourcesInfo[type].count.text = resources[type].ToString();
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
