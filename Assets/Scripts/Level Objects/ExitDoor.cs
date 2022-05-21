using UnityEngine;
using System;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 0.7f;

    private Teleporter teleporter;
    private SceneChanger sceneChanger;
    private TriggerZone trigger;

    public bool CanBeUsed { get => canBeUsed; set => canBeUsed = value; }

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.SceneControllerTag).GetComponent<Teleporter>();
        sceneChanger = GetComponent<SceneChanger>();
        trigger = GetComponent<TriggerZone>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && trigger.IsTriggered && canBeUsed && teleporter.State == Teleporter.States.Stayed) 
        {
            void action()
            {
                sceneChanger.ChangeScene();
            }

            ServiceInfo.CheckpointConditionDone = true; // Для обучения
            Player.instanse.SaveToStorage();
            teleporter.Go(Player.instanse.transform.position, action, fadeSpeed);
        }
    }
}
