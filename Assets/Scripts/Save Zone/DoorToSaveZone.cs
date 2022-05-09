using UnityEngine;

public class DoorToSaveZone : MonoBehaviour
{
    [SerializeField] private DoorFromSaveZone doorFromSaveZone;

    private readonly string playerLayer = "Player";

    public void SetDoorFromSaveZone(DoorFromSaveZone value) => doorFromSaveZone = value;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.E) && collision.CompareTag(playerLayer))
        {
            collision.transform.position = doorFromSaveZone.transform.position;
            doorFromSaveZone.SetExitPosition(transform.position);
            CameraController.instanse.EnableMoving = false;
            CameraController.instanse.transform.position = doorFromSaveZone.CameraPosition;
            collision.GetComponent<SanityController>().DecreasingEnabled = false;
        }
    }
}
