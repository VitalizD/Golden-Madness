using UnityEngine;

public class Minecart : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private GameObject minecart;

    private TriggerZone trigger;
    private PressActionKey pressActionKey;

    public void ActiveMinecart() => minecart.SetActive(true);

    private void Awake()
    {
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Update()
    {
        if (trigger.IsTriggered && Input.GetKeyDown(KeyCode.E) && canBeUsed)
        {
            var backpack = Player.instanse.GetComponent<Backpack>();
            ResourcesSaver.SaveInVillage(backpack.GetAll());
            backpack.Clear();

            // Для обучающего уровня
            ServiceInfo.CheckpointConditionDone = true;

            if (minecart != null)
                minecart.SetActive(false);
            canBeUsed = false;
            pressActionKey.SetActive(false);
        }
    }
}
