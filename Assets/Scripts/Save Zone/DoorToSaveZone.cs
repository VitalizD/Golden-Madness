using UnityEngine;
using System;

public class DoorToSaveZone : MonoBehaviour
{
    [SerializeField] private DoorFromSaveZone doorFromSaveZone;
    [SerializeField] private float stunTime = 2f;

    private Teleporter teleporter;

    private bool isTriggered = false;

    public void SetDoorFromSaveZone(DoorFromSaveZone value) => doorFromSaveZone = value;

    private void Awake()
    {
        teleporter = GameObject.FindGameObjectWithTag(ServiceInfo.SceneControllerTag).GetComponent<Teleporter>();
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
        if (Input.GetKeyDown(KeyCode.E) && isTriggered && teleporter.State == Teleporter.States.Stayed)
        {
            void action()
            {
                CameraController.instanse.EnableMoving = false;
                CameraController.instanse.transform.position = doorFromSaveZone.CameraPosition;
                Player.instanse.GetComponent<SanityController>().DecreasingEnabled = false;
            }

            Player.instanse.SetStun(stunTime);
            doorFromSaveZone.SetExitPosition(transform.position);
            teleporter.Go(doorFromSaveZone.transform.position, action);
        }
    }
}
