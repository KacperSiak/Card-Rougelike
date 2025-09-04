using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardManager : PersistentSingleton<DiscardManager>
{
    [SerializeField] private GameObject _discardPanel;
    private int discardAvailableAmount = 3;

    public bool InUI { get; private set; } = false;

    private void Update()
    {
        if (Input.GetMouseButton(1) && PlayerCardManager.Instance.CardsInHand.Count != 0 && PlayerCardManager.Instance.PlayerDeckInGame.Count != 0)
        {
            CheckForDiscard();
        }
    }

    private void CheckForDiscard()
    {
        if (discardAvailableAmount == 0 || PlayerCardManager.Instance.SelectedCards.Count == 0 || _discardPanel.activeInHierarchy) return;

        _discardPanel.SetActive(true);
        Time.timeScale = 0f;
        InUI = true;
    }

    public void Cancel()
    {
        _discardPanel.SetActive(false);
        Time.timeScale = 1f;
        InUI = false;
    }

    public void Discard()
    {
        if (PlayerCardManager.Instance.SelectedCards.Count > discardAvailableAmount) return;

        _discardPanel.SetActive(false);
        Time.timeScale = 1f;
        InUI = false;

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
