using UnityEngine;

public class ChestLevel : MonoBehaviour
{
    [SerializeField] private ConsumableType[] containedConsumables;
    [SerializeField] private ResourceType[] containedResources;
    [SerializeField] private int minResourcesCount = 1;
    [SerializeField] private int maxResourcesCount = 3;
    [SerializeField] private float chanceOfDropConsumable = 1f;
    [SerializeField] private float chanceOfDropResource = 1f;

    private TriggerZone triggerZone;
    private PressActionKey pressActionKey;
    private SpriteRenderer sprite;

    private bool active = true;

    private void Awake()
    {
        triggerZone = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && active && triggerZone.IsTriggered)
        {
            active = false;
            pressActionKey.SetActive(false);
            sprite.sprite = SpritesStorage.Instanse.OpenedChest;
            var random = Random.Range(0f, chanceOfDropResource + chanceOfDropConsumable);
            if (random >= chanceOfDropResource)
                GetRandomConsumable();
            else
                GetRandomResource();
        }
    }

    private void GetRandomConsumable()
    {
        var consumables = Player.Instanse.GetComponent<Consumables>();
        consumables.Add(containedConsumables[Random.Range(0, containedConsumables.Length)], 1);
    }

    private void GetRandomResource()
    {
        var backpack = Player.Instanse.GetComponent<Backpack>();
        var randomResourcesCount = Random.Range(minResourcesCount, maxResourcesCount + 1);
        if (backpack.CurrentFullness + randomResourcesCount > backpack.MaxCapacity)
        {
            GetRandomConsumable();
            return;
        }    
        var randomResource = containedResources[Random.Range(0, containedResources.Length)];
        backpack.Add(randomResource, randomResourcesCount);
        TextMessagesQueue.Instanse.Add(
            $"{Backpack.GetResourceName(randomResource)} x{randomResourcesCount}",
            SpritesStorage.Instanse.GetResource(randomResource));
    }
}
