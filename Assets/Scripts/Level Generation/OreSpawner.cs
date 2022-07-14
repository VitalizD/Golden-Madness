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

        Instantiate(oresPrefabs[ServiceInfo.GetIndexByChancesArray(spawnChances)], transform.position, Quaternion.identity, transform.parent);
        Destroy(gameObject);
    }
}
