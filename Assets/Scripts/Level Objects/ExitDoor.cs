using UnityEngine;
using System;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 0.7f;

    private bool isTriggered = false;

    private Teleporter teleporter;
    private SceneChanger sceneChanger;

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
        if (isTriggered && Input.GetKeyDown(KeyCode.E) && teleporter.State == Teleporter.States.Stayed) 
        {
            void action()
            {
                sceneChanger.ChangeScene();
            }

            teleporter.Go(Player.instanse.transform.position, action, fadeSpeed);
        }
    }
}
