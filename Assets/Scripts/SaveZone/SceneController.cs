using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private Vector3 position;
    [SerializeField] private string levelToLoad;
    [SerializeField] private VectorValue playerStorage;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.E) && collision.CompareTag("Player"))
        {
            playerStorage.initialValue = position;
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
