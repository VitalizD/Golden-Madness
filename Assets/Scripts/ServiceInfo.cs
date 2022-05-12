public static class ServiceInfo
{
    public static string GroundLayerName { get; } = "Ground";
    public static string EnemiesLayerName { get; } = "Enemies";
    public static string PlayerLayerName { get; } = "Player";

    public static string PlayerTag { get; } = "Player";
    public static string SceneControllerTag { get; } = "Scene Controller";

    public static string MainCanvasName { get; } = "Canvas";
    public static string BlackFilterName { get; } = "Black Filter";

    public static int ChildIndexOfDialogWindow { get; } = 2;

    public static bool CheckpointConditionDone { get; set; } = false;
}
