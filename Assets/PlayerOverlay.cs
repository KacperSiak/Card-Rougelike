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
}
