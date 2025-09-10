using UnityEngine;

public static class SuitRules
{
    public static bool CombineBlackSuits { get; set; }
    public static bool CombineRedSuits { get; set; }

    public static bool AreEqual(Suits a, Suits b)
    {
        if (a == Suits.Joker || b == Suits.Joker) return true;
        if(a == RoundManager.Instance.TrumpSuit || b == RoundManager.Instance.TrumpSuit) return true; //trump suit logic 

        if (CombineBlackSuits)
        {
            bool aBlack = a == Suits.Clubs || a == Suits.Spades;
            bool bBlack = b == Suits.Clubs || b == Suits.Spades;
            if(aBlack && bBlack) return true;
        }

        if (CombineRedSuits)
        {
            bool aRed = a == Suits.Diamonds || a == Suits.Hearts;
            bool bRed = b == Suits.Diamonds || b == Suits.Hearts;
            if (aRed && bRed) return true;
        }

        return a == b;
    }
}
