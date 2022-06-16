using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] objects;

    private void Start()
    {
        var randomIndex = Random.Range(0, objects.Length);
        Instantiate(objects[randomIndex], transform.position, Quaternion.identity, transform);
    }
}
