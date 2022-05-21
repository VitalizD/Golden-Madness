using System.Collections.Generic;

public static class DataStorage
{
    private static Dictionary<ResourceTypes, int> resources = new Dictionary<ResourceTypes, int>();

    public static int FuelTanksCount { get; set; } = 0;
    public static int GrindstonesCount { get; set; } = 0;
    public static int HealthPacksCount { set; get; } = 0;
    public static int SmokingPipesCount { get; set; } = 0;

    public static int BackpackCapacity { get; set; } = 100;
    public static int MaxEnemyDamage { get; set; } = 10;
    public static float MaxTileDamage { get; set; } = 1f;
    public static float HitDamageToPickaxe { get; set; } = 0.5f;

    public static int SleepingBagHealthRecovery { get; set; } = 20;
    public static float SleepingBagSanityRecovery { get; set; } = 50f;

    public static Dictionary<ResourceTypes, int> Resources
    {
        get => new Dictionary<ResourceTypes, int>(resources);
        set => resources = new Dictionary<ResourceTypes, int>(value);
    }
}
