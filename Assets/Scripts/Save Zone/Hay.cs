using UnityEngine;
using System;

public class Hay : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private float fadeSpeed = 0.7f;
    [SerializeField] private SFX haySFX;

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
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag).GetComponent<Teleporter>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && trigger.IsTriggered && canBeUsed)
        {
            haySFX.Play();
            void action()
            {
                Player.Instanse.Sleep();
                Player.Instanse.Say("Tеперь я чувствую себя бодрым", 4);

                ServiceInfo.CheckpointConditionDone = true; // Для обучающего уровня
            }

            CanBeUsed = false;
            teleporter.Go(Player.Instanse.transform.position, action, fadeSpeed);
        }
    }
}
