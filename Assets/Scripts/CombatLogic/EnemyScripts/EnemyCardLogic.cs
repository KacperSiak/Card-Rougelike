using UnityEngine;

public class EnemyCardLogic : MonoBehaviour
{
    public CardsSOTest Card { get; private set; }

    public void SetCardProperties(CardsSOTest card)
    {
        Card = card;
    }


}
