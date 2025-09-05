using UnityEngine;
using System;
using TMPro;

public class PlayerOverlay : PersistenSingleton<PlayerOverlay>
{
    public int gold;
    public float maxHP;
    public float currentHP;
    public float addDamage;
    public int rollTickets;
    public int discardCount = 3;
    [SerializeField] private TextMeshProUGUI _text;

    protected override void OnSingletonAwake() { }

    protected override void OnDestroy()
    {
        base.OnDestroy();
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
