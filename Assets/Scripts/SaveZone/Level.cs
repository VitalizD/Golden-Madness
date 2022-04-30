using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    public Vector3 position;
    public int levelToLoad;
    public VectorValue playerStorage;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.E) && collision.tag.Equals("Player"))
        {
            playerStorage.initialValue = position;
            SceneManager.LoadScene(levelToLoad);
        }
    }
}
