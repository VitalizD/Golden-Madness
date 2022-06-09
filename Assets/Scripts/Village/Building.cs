using UnityEngine;
using System;

public class Building : MonoBehaviour, IStorage
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private bool loadLevel = true;
    [SerializeField] private float buildingFadeSpeed = 1f;
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
        if (Input.GetKeyDown(KeyCode.E) && isTriggered && !maxLevel && canBeUsed)
        {
            if (CanUpgrade())
            {
                Upgrade();
            }
            else
                Player.instanse.Say("Не хватает ресурсов", 3f);
        }
    }

    private void Upgrade()
    {
        void action()
        {
            ++currentLevel;
            actionAfterUpgrading?.Invoke();

            if (!maxLevel)
                upgradeWindow.Show();

            VillageController.instanse.AddResource(requiredResource, -requiredCount);
            VillageController.instanse.Save();
            Save();
            UpdateInfo();
        }

        if (currentLevel == 0)
        {
            CanBeUsed = false;
            upgradeWindow.Hide();
            Teleporter.instanse.Go(Player.instanse.transform.position, action, buildingFadeSpeed, () => CanBeUsed = true);
        }
        else
            action();
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

    private void UpdateUpgradeWindow(string description, ResourceTypes requiredResource, int count, Action actionAfterUpgrading)
    {
        upgradeWindow.SetDescription(description);
        SetRequiredResource(requiredResource, count);
        this.actionAfterUpgrading = actionAfterUpgrading;
    }

    private void UpdateInfo()
    {
        if (currentLevel >= 1)
        {
            upgradeWindow.SetDescriptionAlignFlush();
            upgradeWindow.SetLevel(currentLevel);
            if (constructedBuildingSprite != null)
                sprite.sprite = constructedBuildingSprite;
        }
        else
            upgradeWindow.SetDescriptionAlignLeft();

        upgradeWindow.SetUpgradeActive(currentLevel >= 1);

        if (!maxLevel)
        {
            if (currentLevel == 0)
                upgradeWindow.SetAction("Построить");

            var prefix = $"<b><color=green>";
            var postfix = "</color></b>";
            switch (buildingType)
            {
                case BuildingType.Forge:
                    upgradeWindow.SetTitle("Кузница");
                    upgradeWindow.SetToolIcon(SpritesStorage.instanse.Pickaxe);
                    if (currentLevel > 0)
                        upgradeWindow.SetAction("Улучшить кирку");
                    switch (currentLevel)
                    {
                        case 0:
                            UpdateUpgradeWindow("Позволяет улучшать кирку", ResourceTypes.GoldOre, 3, () => ServiceInfo.CheckpointConditionDone = true);
                            break;
                        case 1:
                            {
                                void action()
                                {
                                    ServiceInfo.CheckpointConditionDone = true; // Для обучения
                                    Player.instanse.AddMaxTileDamage(10);
                                }
                                UpdateUpgradeWindow($"Скорость {prefix}+10%{postfix}", ResourceTypes.GoldOre, 3, action);
                                break;
                            }
                        case 2: UpdateUpgradeWindow($"Урон {prefix}+10%{postfix}", ResourceTypes.IronOre, 10, () => Player.instanse.AddMaxEnemyDamage(10)); break;
                        case 3: UpdateUpgradeWindow($"Прочность {prefix}+10%{postfix}", ResourceTypes.GoldOre, 15, () => Player.instanse.AddHitDamageToPickaxe(-10)); break;
                        case 4: UpdateUpgradeWindow($"Скорость {prefix}+20%{postfix}", ResourceTypes.IronOre, 20, () => Player.instanse.AddMaxTileDamage(20)); break;
                        case 5: UpdateUpgradeWindow($"Урон {prefix}+20%{postfix}", ResourceTypes.GoldOre, 25, () => Player.instanse.AddMaxEnemyDamage(20)); break;
                        case 6: UpdateUpgradeWindow($"Прочность {prefix}+20%{postfix}", ResourceTypes.IronOre, 30, () => Player.instanse.AddHitDamageToPickaxe(-20)); break;
                        case 7: UpdateUpgradeWindow($"Скорость {prefix}+30%{postfix}", ResourceTypes.GoldOre, 35, () => Player.instanse.AddMaxTileDamage(30)); break;
                        case 8: UpdateUpgradeWindow($"Урон {prefix}+30%{postfix}", ResourceTypes.IronOre, 40, () => Player.instanse.AddMaxEnemyDamage(30)); break;
                        case 9: UpdateUpgradeWindow($"Прочность {prefix}+30%{postfix}", ResourceTypes.IronOre, 45, () => Player.instanse.AddHitDamageToPickaxe(-30)); break;
                        case 10:
                            {
                                void action()
                                {
                                    Player.instanse.AddMaxTileDamage(50);
                                    Player.instanse.AddHitDamageToPickaxe(-50);
                                    Player.instanse.AddMaxEnemyDamage(50);
                                }
                                UpdateUpgradeWindow($"Прочность {prefix}+50%{postfix}\nСкорость {prefix}+50%{postfix}\nУрон {prefix}+50%{postfix}", ResourceTypes.IronOre, 50, action);
                                break;
                            }
                        case 11:
                            upgradeWindow.Hide();
                            maxLevel = true;
                            break;
                    }
                    break;

                case BuildingType.Workshow:
                    upgradeWindow.SetTitle("Мастерская");
                    upgradeWindow.SetToolIcon(SpritesStorage.instanse.Lamp);
                    upgradeWindow.SetDescriptionAlignLeft();
                    if (currentLevel > 0)
                        upgradeWindow.SetAction("Улучшить лампу");

                    var pprefix = $"Время горения\n{prefix}+";
                    var ppostfix = $"%{postfix}";
                    void WorkshopUpgradeEffect() => Player.instanse.AddTimeDecreaseValue(0.5f);

                    switch (currentLevel)
                    {
                        case 0: UpdateUpgradeWindow("Позволяет улучшать лампу", ResourceTypes.Coal, 10, null); break;
                        case 1: UpdateUpgradeWindow($"{pprefix}10{ppostfix}", ResourceTypes.Quartz, 15, WorkshopUpgradeEffect); break;
                        case 2: UpdateUpgradeWindow($"{pprefix}20{ppostfix}", ResourceTypes.Coal, 20, WorkshopUpgradeEffect); break;
                        case 3: UpdateUpgradeWindow($"{pprefix}30{ppostfix}", ResourceTypes.Quartz, 25, WorkshopUpgradeEffect); break;
                        case 4: UpdateUpgradeWindow($"{pprefix}40{ppostfix}", ResourceTypes.Coal, 30, WorkshopUpgradeEffect); break;
                        case 5: UpdateUpgradeWindow($"{pprefix}50{ppostfix}", ResourceTypes.Quartz, 35, WorkshopUpgradeEffect); break;
                        case 6: UpdateUpgradeWindow($"{pprefix}60{ppostfix}", ResourceTypes.Coal, 40, WorkshopUpgradeEffect); break;
                        case 7:UpdateUpgradeWindow($"{pprefix}70{ppostfix}", ResourceTypes.Quartz, 45, WorkshopUpgradeEffect);break;
                        case 8: UpdateUpgradeWindow($"{pprefix}80{ppostfix}", ResourceTypes.Coal, 50, WorkshopUpgradeEffect); break;
                        case 9: UpdateUpgradeWindow($"{pprefix}90{ppostfix}", ResourceTypes.Quartz, 55, WorkshopUpgradeEffect); break;
                        case 10: UpdateUpgradeWindow($"{pprefix}100{ppostfix}", ResourceTypes.Coal, 60, WorkshopUpgradeEffect); break;
                        case 11:
                            upgradeWindow.Hide();
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
