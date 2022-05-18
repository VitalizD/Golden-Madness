using System.Collections.Generic;

public static class FromLevelToVillage
{
    private static Dictionary<ResourceTypes, int> resources;

    public static Dictionary<ResourceTypes, int> Resources
    {
        get => new Dictionary<ResourceTypes, int>(resources);
        set => resources = new Dictionary<ResourceTypes, int>(value);
    }
}
