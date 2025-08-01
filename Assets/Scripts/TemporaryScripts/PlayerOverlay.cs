using UnityEngine;
using System;
using TMPro;

public class PlayerOverlay : MonoBehaviour
{
    public int gold;
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
        else
        {
            Debug.LogWarning("Found another PlayerOverlay");
        }
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

    public void AddHealth(int health)
    {
        currentHP += health;
        if (currentHP > maxHP)
        {
            // show maxhp/maxhp
        }
        else
        {
            // show currentHealth/maxhp
        }
    }

    public void LoseHealth(int health)
    {
        currentHP -= health;
        // show new hp 
    }

    public void SetMaxHP(float newHP)
    {
        float hpPercentege = newHP / maxHP;
        maxHP = newHP;
        currentHP = (float)Math.Round(hpPercentege * currentHP);
        // show current and max hp
    }
}
