using UnityEngine;

public class Minecart : MonoBehaviour
{
    private TriggerZone trigger;

    private void Awake()
    {
        trigger = GetComponent<TriggerZone>();
    }

    private void Update()
    {
        if (trigger.IsTriggered && Input.GetKeyDown(KeyCode.E))
        {
            var backpack = Player.instanse.GetComponent<Backpack>();
            ResourcesSaver.SaveInVillage(backpack.GetAll());
            backpack.Clear();

            // Для обучающего уровня
            ServiceInfo.CheckpointConditionDone = true;

            gameObject.SetActive(false);
        }
    }
}
