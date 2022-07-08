using UnityEngine;

public class PressActionKey : MonoBehaviour
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private int childIndexE = 0;

    private GameObject e;

    public void Show() => e.SetActive(true);

    public void SetActive(bool value)
    {
        isActive = value;
        if (!value)
            e.SetActive(false);
    }

    private void Awake()
    {
        e = transform.GetChild(childIndexE).gameObject;
        e.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.GetComponent<Player>() != null)
            e.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            e.SetActive(false);
    }
}
