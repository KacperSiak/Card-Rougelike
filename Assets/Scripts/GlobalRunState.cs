public static class GlobalRunState
{
    public static bool PendingDefeatRegeneration = false;

    public static bool PendingMapReloadAfterDefeat = false;
    public static bool UseFixedSeedOnNextMap = false;

    public static void FlagDefeatReload()
    {
        PendingMapReloadAfterDefeat = true;
        UseFixedSeedOnNextMap = true;
        PendingDefeatRegeneration = false;
    UnityEngine.Debug.Log("Defeat — preparing map reload (seed=0).");
    }

    public static void ClearReloadFlags()
    {
        PendingMapReloadAfterDefeat = false;
        UseFixedSeedOnNextMap = false;
    }
}
