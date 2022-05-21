using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    private bool isTriggered = false;

    public bool IsTriggered { get => isTriggered; }

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
}
