using UnityEngine;
using System.Linq;

public class OreSpawner : MonoBehaviour
{
    private void Start()
    {
        var levelGen = LevelGeneration.Instanse;
        if (levelGen == null)
            return;

        var random = Random.Range(0f, 1f);
        if (random > levelGen.SpawnOreChance)
            return;

        var spawnChances = levelGen.SpawnChances;
        var oresPrefabs = levelGen.OrePrefabs;

        var sum = spawnChances.Sum();
        var current = 0f;
        random = Random.Range(0f, sum);

        for (var i = 0; i < oresPrefabs.Length; ++i)
        {
            current += spawnChances[i];
            if (current >= random)
            {
                Instantiate(oresPrefabs[i], transform.position, Quaternion.identity, transform.parent);
                Destroy(gameObject);
                break;
            }
        }
    }
}
