using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerOverlay : MonoBehaviour
{
    public float Gold;
    public float maxHP;
    public float currentHP;
    public float addDamage;
    public List<NormalCard> deckList;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeReference] public List<ConsumableCards> consumables;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void AddNormalCard(NormalCard card)
    {
        deckList.Add(card);
    }

    public void RemoveNormalCard(NormalCard card)
    {
        deckList.Remove(card);
    }
}
