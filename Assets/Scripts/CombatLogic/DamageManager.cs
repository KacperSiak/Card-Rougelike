using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
   public static DamageManager Instance { get; private set; }

    public Dictionary<int, List<GameObject>> eCards;
    public List<GameObject> pCards;
    public List<GameObject> cardHolders;

    private int enemyPower;
   private int playerPower;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            eCards = new Dictionary<int, List<GameObject>>();
            pCards = new List<GameObject>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCardHolder(int index, ComboTypes type)
    { 
        cardHolders[index].GetComponent<CardHolderLogic>().SetProperties(type);
    }

    public void CalculateDamage()
    {
        
    }

}
