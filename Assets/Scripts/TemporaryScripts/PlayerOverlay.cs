using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerOverlay : MonoBehaviour
{
    public float gold;
    public float maxHP;
    public float currentHP;
    public float addDamage;
    public int rollTickets;
    public List<NormalCard> deckList;
    [SerializeField] private TextMeshProUGUI _text;

    public static PlayerOverlay instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Found another PlayerOverlay");
        }
    }
    public void AddNormalCard(NormalCard card)
    {
        deckList.Add(card);
    }

    public void RemoveNormalCard(NormalCard card)
    {
        deckList.Remove(card);
    }

    public void SpendGold(int gold)
    {
        this.gold -= gold;
        // wy�wietl nowe siano :)
    }
    public void AddGold(int gold)
    {
        this.gold += gold;
        // wy�wietl nowe siano :)
    }
}
