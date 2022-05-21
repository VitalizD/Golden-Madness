using UnityEngine;
using System;

public class Building : MonoBehaviour, IStorage
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private bool loadLevel = true;
    [SerializeField] private BuildingType buildingType;
    [SerializeField] private Sprite constructedBuildingSprite;

    private UpgradeWindow upgradeWindow;
    private SpriteRenderer sprite;

    private bool isTriggered = false;
    private bool maxLevel = false;
    private int currentLevel = 0;
    private int requiredCount = 0;
    private ResourceTypes requiredResource = ResourceTypes.None;
    private Action actionAfterUpgrading = null;

    public bool CanBeUsed { get => canBeUsed; set => canBeUsed = value; }

    public void Save()
    {
        PlayerPrefs.SetInt(buildingType.ToString() + PlayerPrefsKeys.CurrentLevelOfBuildingPostfix, currentLevel);
    }

    public void Load()
    {
        currentLevel = PlayerPrefs.GetInt(buildingType.ToString() + PlayerPrefsKeys.CurrentLevelOfBuildingPostfix, 0);
    }

    private void Awake()
    {
        upgradeWindow = transform.GetChild(0).GetComponent<UpgradeWindow>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (loadLevel)
            Load();

        UpdateInfo();
        upgradeWindow.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null && canBeUsed)
        {
            isTriggered = true;
            if (!maxLevel)
                upgradeWindow.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null && canBeUsed)
        {
            isTriggered = false;
            if (!maxLevel)
                upgradeWindow.Hide();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTriggered && !maxLevel)
        {
            if (CanUpgrade())
            {
                VillageController.instanse.AddResource(requiredResource, -requiredCount);
                VillageController.instanse.Save();
                Upgrade();
                Save();
            }
            else
                Player.instanse.Say("Не хватает ресурсов", 3f);
        }
    }

    private void Upgrade()
    {
        ++currentLevel;
        actionAfterUpgrading?.Invoke();
        UpdateInfo();

        if (!maxLevel)
            upgradeWindow.Show();
    }

    private void SetRequiredResource(ResourceTypes type, int count)
    {
        upgradeWindow.SetRequiredResource(type, count);
        requiredResource = type;
        requiredCount = count;
    }

    private bool CanUpgrade()
    {
        return VillageController.instanse.GetResourcesCount(requiredResource) >= requiredCount;
    }

    private void UpdateInfo()
    {
        if (currentLevel >= 1 && constructedBuildingSprite != null)
            sprite.sprite = constructedBuildingSprite;

        if (!maxLevel)
        {
            if (currentLevel == 0)
                upgradeWindow.SetAction("- Построить");
            else
                upgradeWindow.SetAction("- Улучшить");

            switch (buildingType)
            {
                case BuildingType.Forge:
                    upgradeWindow.SetTitle("Кузница");
                    switch (currentLevel)
                    {
                        case 0:
                            upgradeWindow.SetDescription("Улучшает кирку");
                            SetRequiredResource(ResourceTypes.GoldOre, 3);
                            actionAfterUpgrading = () => { ServiceInfo.CheckpointConditionDone = true; }; // Для обучения
                            break;
                        case 1:
                            upgradeWindow.SetDescription("Увеличивает скорость добычи, урон и прочность кирки на 20%");
                            SetRequiredResource(ResourceTypes.GoldOre, 3);
                            actionAfterUpgrading = () =>
                            {
                                ServiceInfo.CheckpointConditionDone = true; // Для обучения

                                Player.instanse.AddHitDamageToPickaxe(-20);
                                Player.instanse.AddMaxEnemyDamage(20);
                                Player.instanse.AddMaxTileDamage(20);
                                upgradeWindow.Hide();
                            };
                            break;
                        case 2:
                            maxLevel = true;
                            break;
                    }
                    break;
            }
        }
    }
}
