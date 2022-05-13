using UnityEngine;

public class Minecart : MonoBehaviour
{
    private bool isTriggered = false;

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
            var backpack = Player.instanse.GetComponent<Backpack>();
            // ����������� �������� �� ������� � ���������
            backpack.Clear();

            // ��� ���������� ������
            ServiceInfo.CheckpointConditionDone = true;

            Destroy(gameObject);
        }
    }
}
