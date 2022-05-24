public static class ServiceInfo
{
    public static string GroundLayerName { get; } = "Ground";
    public static string EnemiesLayerName { get; } = "Enemies";
    public static string PlayerLayerName { get; } = "Player";

    public static string PlayerTag { get; } = "Player";
    public static string BlackFilterTag { get; } = "Black Filter";
    public static string HayTag { get; } = "Hay";
    public static string MinecartTag { get; } = "Minecart";
    public static string ChestTag { get; } = "Chest";

    public static string MainCanvasName { get; } = "Canvas";
    public static string BlackFilterName { get; } = "Black Filter";

    public static string VillageScene { get; } = "Village";
    public static string TutorialLevel { get; } = "TutorialLevel";

    public static int ChildIndexOfDialogWindow { get; } = 2;
    public static int ChildIndexOfLamp { get; } = 3;

    public static bool CheckpointConditionDone { get; set; } = false;
    public static bool TutorialDone { get; set; } = true;
}
