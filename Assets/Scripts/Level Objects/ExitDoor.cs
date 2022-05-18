using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 0.7f;

    private bool isTriggered = false;

    private Teleporter teleporter;

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
        if (isTriggered && Input.GetKeyDown(KeyCode.E) && teleporter.State == Teleporter.States.Stayed) 
        {
            void action()
            {
                // Переход в поселение
            }

            teleporter.Go(Player.instanse.transform.position, action, fadeSpeed);
        }
    }
}
