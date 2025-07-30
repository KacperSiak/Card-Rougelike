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
    [SerializeField] private TextMeshProUGUI _text;

    public static PlayerOverlay instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SpendGold(int gold)
    {
        this.gold -= gold;
        // wyœwietl nowe siano :)
    }
    public void AddGold(int gold)
    {
        this.gold += gold;
        // wyœwietl nowe siano :)
    }
}
