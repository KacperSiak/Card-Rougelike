using UnityEngine;

public class BuffsAndDebuffsManager : MonoBehaviour
{
    public static BuffsAndDebuffsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("I found another Buff and Debuff Menager");
        }
    }

    public void ApplyBuff(PotionData potion)
    {
        var ctx = new PotionContext
        {
            player = PlayerOverlay.instance
        };
        foreach (var effect in potion.potionEffects)
        {
            effect.Register(ctx, potion.fightsLeft);
        }
    }

    // public void ApplyDebuff()
}
