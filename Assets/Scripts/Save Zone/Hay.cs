using UnityEngine;
using System;

public class Hay : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 0.7f;

    private PlayerDialogWindow dialogWindow;
    private Teleporter teleporter;

    private bool isTriggered = false;

    public void SetCanBeUsedTrue() => canBeUsed = true;

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.SceneControllerTag).GetComponent<Teleporter>();
    }

    private void Start()
    {
        dialogWindow = Player.instanse.transform.GetChild(ServiceInfo.ChildIndexOfDialogWindow).GetComponent<PlayerDialogWindow>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            isTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(ServiceInfo.PlayerTag))
            isTriggered = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isTriggered && canBeUsed)
        {
            void action()
            {
                Player.instanse.Sleep();
                dialogWindow.gameObject.SetActive(true);
                dialogWindow.Show("Tеперь я чувствую себя бодрым", 4);
                canBeUsed = false;

                ServiceInfo.CheckpointConditionDone = true; // Для обучающего уровня
            }

            teleporter.Go(Player.instanse.transform.position, action, fadeSpeed);
        }
    }
}
