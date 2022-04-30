using UnityEngine;

public class HideInfoWindowTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        InformationWindow.instance.Hide();
        Destroy(gameObject);
    }
}
