using UnityEngine;

public static class SceneBlockState
{
    public static bool ShopUsed { get; private set; }
    public static bool HealUsed { get; private set; }

    public static void Block(string sceneKey)
    {
        if (sceneKey == "Shop") ShopUsed = true;
        if (sceneKey == "Heal") HealUsed = true;
    }

    public static bool IsBlocked(string sceneName)
    {
    if (sceneName == "HealingDesign" && HealUsed) return false;
        if (sceneName == "ShopScene" && ShopUsed) return false;
        return true;
    }
}
