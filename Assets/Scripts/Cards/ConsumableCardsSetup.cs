using UnityEngine;
using System.Collections.Generic;

public class ConsumableCardsSetup : MonoBehaviour
{
    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _cardHand;

    private void Awake()
    {
        RoundManager.OnCombatStart += SetupCards;
    }

    private void SetupCards()
    {
        List<ConsumablesData> consCards = ConsumablesAndRelicMenager.Instance.currentConsums;

        foreach (ConsumablesData Card in consCards)
        {
            var set = _cardPrefab.GetComponent<ConsCardProprties>();
            if (set != null)
            {
                set.Setup(Card);
            }
        }
    }
}
