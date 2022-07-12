using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private void Start()
    {
        var levelGen = LevelGeneration.Instanse;
        if (levelGen == null)
            return;

        Instantiate(levelGen.GetRandomEnemy(), transform.position, Quaternion.identity, transform);
    }
}
