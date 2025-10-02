using UnityEngine;
using System.Collections.Generic;

public class ConsumableCardsSetup : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabs = new List<GameObject>();

    private void Awake()
    {
        RoundManager.OnCombatStart += SetupCards;
    }

    private void SetupCards()
    {
        List<ConsumablesData> consCards = ConsumablesAndRelicMenager.Instance.currentConsums;

        for (int i = 0; i<consCards.Count; i++)
        {
            var set = _prefabs[i].GetComponent<ConsCardProperties>();
            set.Setup(consCards[i]);
        }
    }
}
