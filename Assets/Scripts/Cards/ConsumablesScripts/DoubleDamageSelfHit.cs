using System;

[Serializable]
public class DoubleDamageSelfHit : CardEffect
{
    public override void ApplyEffect(ConsumableContext ctx)
    {
        // you deal double damage next/this fight
        // you get 10% damage of your attacks
    }
}
