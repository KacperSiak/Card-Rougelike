using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class DiscardManager : MonoBehaviour
{
    private int discardAvailableAmount;

    private void OnEnable()
    {
        discardAvailableAmount = PlayerOverlay.Instance.discardCount; // Kacper Added
    }

    public void Discard()
    {
        if (PlayerCardManager.Instance.SelectedCards.Count > discardAvailableAmount || PlayerCardManager.Instance.SelectedCards.Count == 0 || PlayerCardManager.Instance._playerDeckInGame.Count == 0) return;

        discardAvailableAmount -= PlayerCardManager.Instance.SelectedCards.Count;

        foreach (var card in PlayerCardManager.Instance.SelectedCards)
        {
            card.GetComponent<PlayerCardLogic>().ResetCard();
            card.SetActive(false);
            ObjPoolManager.Instance.ReturnCard(card);
        }

        PlayerCardManager.Instance.UpdateAfterDiscard(amount: PlayerCardManager.Instance.SelectedCards.Count);
    }
}
