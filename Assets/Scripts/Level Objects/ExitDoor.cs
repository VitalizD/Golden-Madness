using UnityEngine;
using System;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 0.7f;

    private bool isTriggered = false;

    private Teleporter teleporter;
    private SceneChanger sceneChanger;

    public bool CanBeUsed { get => canBeUsed; set => canBeUsed = value; }

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.SceneControllerTag).GetComponent<Teleporter>();
        sceneChanger = GetComponent<SceneChanger>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            isTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            isTriggered = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTriggered && canBeUsed && teleporter.State == Teleporter.States.Stayed) 
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
