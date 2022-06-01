using UnityEngine;
using System;

public class DoorToSaveZone : MonoBehaviour
{
    [SerializeField] private bool canBeUsed = true;
    [SerializeField] private DoorFromSaveZone doorFromSaveZone;
    [SerializeField] private float fadeSpeed = 1.2f;

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

    public void SetDoorFromSaveZone(DoorFromSaveZone value) => doorFromSaveZone = value;

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.BlackFilterTag).GetComponent<Teleporter>();
        trigger = GetComponent<TriggerZone>();
        pressActionKey = GetComponent<PressActionKey>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canBeUsed && trigger.IsTriggered && teleporter.State == Teleporter.States.Stayed)
        {
            void action()
            {
                CameraController.instanse.EnableMoving = false;
                CameraController.instanse.transform.position = doorFromSaveZone.CameraPosition;
                CameraController.instanse.Size = doorFromSaveZone.CameraSizeInSaveZone;
                Player.instanse.GetComponent<SanityController>().DecreasingEnabled = false;
            }

            //CanBeUsed = false;
            doorFromSaveZone.Refresh(transform.position);
            teleporter.Go(doorFromSaveZone.transform.position, action, fadeSpeed);
        }
    }
}
