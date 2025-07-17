using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
   public static DamageManager Instance { get; private set; }

    private int enemyPower;
    private int playerPower;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CalculateDamage()
    {
        
    }

}
