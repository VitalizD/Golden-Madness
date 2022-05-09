using UnityEngine;

public class DoorToSaveZone : MonoBehaviour
{
    [SerializeField] private DoorFromSaveZone doorFromSaveZone;

    private bool isTriggered = false;

    public void SetDoorFromSaveZone(DoorFromSaveZone value) => doorFromSaveZone = value;

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
        if (isTriggered && Input.GetKeyDown(KeyCode.E))
        {
            Player.instanse.transform.position = doorFromSaveZone.transform.position;
            doorFromSaveZone.SetExitPosition(transform.position);
            CameraController.instanse.EnableMoving = false;
            CameraController.instanse.transform.position = doorFromSaveZone.CameraPosition;
            Player.instanse.GetComponent<SanityController>().DecreasingEnabled = false;
        }
    }
}
