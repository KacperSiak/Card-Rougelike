using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DiscardManager : PersistenSingleton<DiscardManager>
{
    [SerializeField] private GameObject _discardPanel;
    [SerializeField] private PlayerCardsAnimation _playerCardsAnimation;
    private int discardAvailableAmount = 3;

    public static UnityEvent OnDiscard = new UnityEvent();

    protected override void OnSingletonAwake()
    {
        //discardAvailableAmount = PlayerManager.Instance.availableDiscards;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

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
        RoundManager.Instance.InUI = true;
    }

    public void Cancel()
    {
        _discardPanel.SetActive(false);
        Time.timeScale = 1f;
        RoundManager.Instance.InUI = false;
    }

    public void Discard()
    {
        if (PlayerCardManager.Instance.SelectedCards.Count > discardAvailableAmount) return;

        _discardPanel.SetActive(false);
        Time.timeScale = 1f;
        RoundManager.Instance.InUI = false;

        discardAvailableAmount -= PlayerCardManager.Instance.SelectedCards.Count;

        _playerCardsAnimation.RemoveCard(PlayerCardManager.Instance.SelectedCards.ConvertAll(card => card.transform).ToArray());

        OnDiscard?.Invoke();

        PlayerCardManager.Instance.UpdateAfterDiscard(amount: PlayerCardManager.Instance.SelectedCards.Count);
    }
}
