using UnityEngine;
using System;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 0.7f;

    private Teleporter teleporter;
    private SceneChanger sceneChanger;
    private TriggerZone trigger;
    private PressActionKey pressActionKey;

    public bool CanBeUsed
    {
        get => canBeUsed;
        set
        {
            canBeUsed = value;
            pressActionKey.SetActive(value);
        }
    }

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.SceneControllerTag).GetComponent<Teleporter>();
        sceneChanger = GetComponent<SceneChanger>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && trigger.IsTriggered && canBeUsed && teleporter.State == Teleporter.States.Stayed) 
        {
            void action()
            {
                sceneChanger.ChangeScene();
            }

            CanBeUsed = false;
            ServiceInfo.CheckpointConditionDone = true; // Для обучения
            Player.instanse.SaveToStorage();
            teleporter.Go(Player.instanse.transform.position, action, fadeSpeed);
        }
    }
}
