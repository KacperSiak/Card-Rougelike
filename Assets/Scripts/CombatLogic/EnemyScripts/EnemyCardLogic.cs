using UnityEngine;

public class EnemyCardLogic : MonoBehaviour
{
    //Remove if I don't need it any more in the future

    public CardsSOTest Card { get; private set; }

    public void SetCardProperties(CardsSOTest card)
    {
        Card = card;
    }


}
