using UnityEngine;

public class EnemyCardLogic : MonoBehaviour
{
    //Remove if I don't need it any more in the future

    public CardsSO Card { get; private set; }

    public void SetCardProperties(CardsSO card)
    {
        Card = card;
    }


}
