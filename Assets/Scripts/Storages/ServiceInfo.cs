using UnityEngine;
using System.Linq;

public static class ServiceInfo
{
    public static string GroundLayerName { get; } = "Ground";
    public static string EnemiesLayerName { get; } = "Enemies";
    public static string PlayerLayerName { get; } = "Player";

    public static string BlackFilterTag { get; } = "Black Filter";
    public static string HayTag { get; } = "Hay";
    public static string MinecartTag { get; } = "Minecart";
    public static string ChestTag { get; } = "Chest";
    public static string GameOverTag { get; } = "Game Over";
    public static string RedFilterTag { get; } = "Red Filter";
    public static string SelectionTag { get; } = "Selection";
    public static string GameplayCanvasTag { get; } = "Gameplay Canvas";

    public static string VillageScene { get; } = "Village";
    public static string TutorialLevel { get; } = "TutorialLevel";

    public static bool CheckpointConditionDone { get; set; } = false;
    public static bool TutorialDoneInCave { get; set; } = false;
    public static bool TutorialDone { get; set; } = true;

    public static int GetIndexByChancesArray(float[] spawnChances)
    {
        var sum = spawnChances.Sum();
        var current = 0f;
        var random = Random.Range(0f, sum);

        for (var i = 0; i < spawnChances.Length; ++i)
        {
            current += spawnChances[i];
            if (current >= random)
                return i;
        }

        return 0;
    }
}
