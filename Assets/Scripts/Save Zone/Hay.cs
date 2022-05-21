using UnityEngine;
using System;

public class Hay : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 0.7f;

    private PlayerDialogWindow dialogWindow;
    private Teleporter teleporter;
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
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Start()
    {
        dialogWindow = Player.instanse.transform.GetChild(ServiceInfo.ChildIndexOfDialogWindow).GetComponent<PlayerDialogWindow>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && trigger.IsTriggered && canBeUsed)
        {
            void action()
            {
                Player.instanse.Sleep();
                dialogWindow.gameObject.SetActive(true);
                dialogWindow.Show("Tеперь я чувствую себя бодрым", 4);

                ServiceInfo.CheckpointConditionDone = true; // Для обучающего уровня
            }

            CanBeUsed = false;
            teleporter.Go(Player.instanse.transform.position, action, fadeSpeed);
        }
    }
}
