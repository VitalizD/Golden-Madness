using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestSZ : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;

    [Tooltip("Количество = индекс + 1\nУкажите части, например, 35, 30, 20 и 15")]
    [SerializeField] private float[] dropCountConsumableChances = new[] { 35f, 30f, 20f, 15f };
    [SerializeField] private SFX openSFX;

    private SpriteRenderer sprite;
    private TriggerZone trigger;
    private Consumables consumables;
    private PressActionKey pressActionKey;
    private Sprite normalSprite;

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);
            sprite.sprite = normalSprite;
        }
    }

    private void Awake()
    {
        openSFX.Position = gameObject.transform.position;
        sprite = GetComponent<SpriteRenderer>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
        normalSprite = sprite.sprite;
    }

    private void Start()
    {
        consumables = Player.Instanse.GetComponent<Consumables>();
    }

    private void Update()
    {
        if (canBeUsed && trigger.IsTriggered && Input.GetKeyDown(KeyCode.E))
        {
            openSFX.Play();
            var consumablesCount = GetConsumablesCount();
            var consumablesToAdd = GetRandomConsumables(consumablesCount);

            foreach (var consumable in consumablesToAdd)
                consumables.Add(consumable.Key, consumable.Value);

            CanBeUsed = false;
            ServiceInfo.CheckpointConditionDone = true; // Для обучения
            sprite.sprite = SpritesStorage.Instanse.OpenedChest;
        }
    }

    private int GetConsumablesCount()
    {
        var sum = dropCountConsumableChances.Sum();
        var random = Random.Range(0f, sum);

        var current = 0f;
        for (var i = 0; i < dropCountConsumableChances.Length; ++i)
        {
            current += dropCountConsumableChances[i];
            if (current >= random)
                return i + 1;
        }

        return 1;
    }

    private Dictionary<ConsumableType, int> GetRandomConsumables(int count)
    {
        var result = new Dictionary<ConsumableType, int>();
        var consumableTypes = (ConsumableType[])System.Enum.GetValues(typeof(ConsumableType));

        // Исключаем противоядие из списка возможных расходников. Временное решение
        consumableTypes = consumableTypes
            .Where(element => element != ConsumableType.Antidote)
            .ToArray();

        for (var i = 0; i < count; ++i)
        {
            var randomConsumable = consumableTypes[Random.Range(0, consumableTypes.Length)];

            if (result.ContainsKey(randomConsumable))
                ++result[randomConsumable];
            else
                result[randomConsumable] = 1;
        }
        return result;
    }
}
