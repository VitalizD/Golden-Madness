using UnityEngine;
using System;

public class Building : MonoBehaviour
{
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

    private void Awake()
    {
        upgradeWindow = transform.GetChild(0).GetComponent<UpgradeWindow>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        UpdateInfo();
        upgradeWindow.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            isTriggered = true;
            if (!maxLevel)
                upgradeWindow.Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
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
                Upgrade();
            }
            else
                Player.instanse.Say("Не хватает ресурсов", 3f);
        }
    }

    private void UpdateInfo()
    {
        if (currentLevel >= 1 && constructedBuildingSprite != null)
            sprite.sprite = constructedBuildingSprite;

        switch (buildingType)
        {
            case BuildingType.Forge:
                upgradeWindow.SetTitle("Кузница");
                switch (currentLevel)
                {
                    case 0:
                        upgradeWindow.SetDescription("Улучшает кирку");
                        upgradeWindow.SetAction("- Построить");
                        SetRequiredResource(ResourceTypes.GoldOre, 3);
                        break;
                    case 1:
                        upgradeWindow.SetDescription("Увеличивает скорость добычи, урон и прочность кирки на 20%");
                        upgradeWindow.SetAction("- Улучшить");
                        SetRequiredResource(ResourceTypes.GoldOre, 3);
                        actionAfterUpgrading = () =>
                        {
                            Player.instanse.AddHitDamageToPickaxe(-20);
                            Player.instanse.AddMaxEnemyDamage(20);
                            Player.instanse.AddMaxTileDamage(20);
                            upgradeWindow.Hide();
                            maxLevel = true;
                        };
                        break;
                }
                break;
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
        if (VillageController.instanse.GetResourcesCount(requiredResource) >= requiredCount)
            return true;
        return false;
    }
}
