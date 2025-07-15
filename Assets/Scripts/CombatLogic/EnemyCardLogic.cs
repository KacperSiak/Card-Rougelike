using UnityEngine;

public class EnemyCardLogic : MonoBehaviour
{
    public int EnemyPower { get; private set; }

    public void SetCardProperties(int power)
    {
        EnemyPower = power;
    }


}
