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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
