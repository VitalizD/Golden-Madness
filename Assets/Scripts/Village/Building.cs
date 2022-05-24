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
                            };
                            break;
                        case 2:
                            upgradeWindow.SetDescription("Увеличивает скорость добычи, урон и прочность кирки на 20%");
                            SetRequiredResource(ResourceTypes.GoldOre, 16);
                            actionAfterUpgrading = () =>
                            {
                                Player.instanse.AddHitDamageToPickaxe(-20);
                                Player.instanse.AddMaxEnemyDamage(20);
                                Player.instanse.AddMaxTileDamage(20);
                            };
                            break;
                        case 3:
                            upgradeWindow.SetDescription("Увеличивает скорость добычи, урон и прочность кирки на 20%");
                            SetRequiredResource(ResourceTypes.GoldOre, 35);
                            actionAfterUpgrading = () =>
                            {
                                Player.instanse.AddHitDamageToPickaxe(-20);
                                Player.instanse.AddMaxEnemyDamage(20);
                                Player.instanse.AddMaxTileDamage(20);
                                upgradeWindow.Hide();
                            };
                            break;
                        case 4:
                            maxLevel = true;
                            break;
                    }
                    break;

                case BuildingType.Workshow:
                    upgradeWindow.SetTitle("Мастерская");
                    switch (currentLevel)
                    {
                        case 0:
                            upgradeWindow.SetDescription("Улучшает лампу");
                            SetRequiredResource(ResourceTypes.Coal, 4);
                            break;
                        case 1:
                            upgradeWindow.SetDescription("Лампа светит на 40% дольше");
                            SetRequiredResource(ResourceTypes.Coal, 6);
                            actionAfterUpgrading = () => Player.instanse.AddFuelDecreaseValue(40);
                            break;
                        case 2:
                            upgradeWindow.SetDescription("Лампа светит на 40% дольше");
                            SetRequiredResource(ResourceTypes.Coal, 15);
                            actionAfterUpgrading = () => Player.instanse.AddFuelDecreaseValue(40);
                            break;
                        case 3:
                            upgradeWindow.SetDescription("Лампа светит на 40% дольше");
                            SetRequiredResource(ResourceTypes.Coal, 30);
                            actionAfterUpgrading = () =>
                            {
                                Player.instanse.AddFuelDecreaseValue(40);
                                upgradeWindow.Hide();
                            };
                            break;
                        case 4:
                            maxLevel = true;
                            break;
                    }
                    break;

                case BuildingType.SleepingBagShop:
                    upgradeWindow.SetTitle("Лавка спальников");
                    switch (currentLevel)
                    {
                        case 0:
                            upgradeWindow.SetDescription("Улучшает спальник");
                            SetRequiredResource(ResourceTypes.Quartz, 7);
                            break;
                        case 1:
                            upgradeWindow.SetDescription("Сон восстанавливает на 30% здоровья и 60% рассудка больше");
                            SetRequiredResource(ResourceTypes.Quartz, 5);
                            actionAfterUpgrading = () =>
                            {
                                Player.instanse.AddSleepingBagHealthRecovery(30);
                                Player.instanse.AddSleepingBagSanityRecovery(60);
                            };
                            break;
                        case 2:
                            upgradeWindow.SetDescription("Сон восстанавливает на 30% здоровья и 60% рассудка больше");
                            SetRequiredResource(ResourceTypes.Quartz, 22);
                            actionAfterUpgrading = () =>
                            {
                                Player.instanse.AddSleepingBagHealthRecovery(30);
                                Player.instanse.AddSleepingBagSanityRecovery(60);
                            };
                            break;
                        case 3:
                            upgradeWindow.SetDescription("Сон восстанавливает на 30% здоровья и 60% рассудка больше");
                            SetRequiredResource(ResourceTypes.Quartz, 38);
                            actionAfterUpgrading = () =>
                            {
                                Player.instanse.AddSleepingBagHealthRecovery(30);
                                Player.instanse.AddSleepingBagSanityRecovery(60);
                                upgradeWindow.Hide();
                            };
                            break;
                        case 4:
                            maxLevel = true;
                            break;
                    }
                    break;

                case BuildingType.SewingWorkshop:
                    upgradeWindow.SetTitle("Швейная мастерская");
                    switch (currentLevel)
                    {
                        case 0:
                            upgradeWindow.SetDescription("Улучшает рюкзак");
                            SetRequiredResource(ResourceTypes.IronOre, 6);
                            break;
                        case 1:
                            upgradeWindow.SetDescription("Увеличивает переносимый вес на 10 едениц");
                            SetRequiredResource(ResourceTypes.IronOre, 5);
                            actionAfterUpgrading = () => Player.instanse.AddBackpackCapacity(10);
                            break;
                        case 2:
                            upgradeWindow.SetDescription("Увеличивает переносимый вес на 10 едениц");
                            SetRequiredResource(ResourceTypes.IronOre, 17);
                            actionAfterUpgrading = () => Player.instanse.AddBackpackCapacity(10);
                            break;
                        case 3:
                            upgradeWindow.SetDescription("Увеличивает переносимый вес на 10 едениц");
                            SetRequiredResource(ResourceTypes.IronOre, 29);
                            actionAfterUpgrading = () =>
                            {
                                Player.instanse.AddBackpackCapacity(10);
                                upgradeWindow.Hide();
                            };
                            break;
                        case 4:
                            maxLevel = true;
                            break;
                    }
                    break;
            }
        }
    }
}
