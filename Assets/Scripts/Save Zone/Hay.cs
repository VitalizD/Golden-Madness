using UnityEngine;

public class Hay : MonoBehaviour
{
    private bool isTriggered = false;
    private PlayerDialogWindow dialogWindow;

    private void Awake()
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
        if (isTriggered && Input.GetKeyDown(KeyCode.E))
        {
            Player.instanse.Sleep();

            dialogWindow.gameObject.SetActive(true);
            dialogWindow.Show("Tеперь я чувствую себя бодрым", 4);

            ServiceInfo.CheckpointConditionDone = true; // Для обучающего уровня

            Destroy(gameObject);
        }
    }
}
