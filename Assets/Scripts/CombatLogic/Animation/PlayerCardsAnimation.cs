using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardsAnimation : MonoBehaviour
{
    [field: SerializeField] public int Interval = 3;
    [SerializeField] private float _duration = 0.1f;
    private Vector3 _idleV = new (0, 0.0001f, 0f); // Idle animation vector
    public bool WaveAnimationActive { get; private set; } = false;

    public static PlayerCardsAnimation Instance { get; private set; }

    [SerializeField] private List<Transform> _newPositions;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartWaveAnimation()
    {
        if(PlayerCardManager.Instance.CardsInHand.Count == 0)
            return;

        WaveAnimationActive = true;
        StartCoroutine(IdleAnimation());
    }

    public void StopWaveAnimation()
    {
        WaveAnimationActive = false;
        StopCoroutine(IdleAnimation());
    }

    private IEnumerator IdleAnimation()
    {
        var card = PlayerCardManager.Instance.CardsInHand[0];
        card.GetComponent<PlayerCardLogic>().SetAnimationPlacement(newPosition: card.transform.position + _idleV, newRotation: card.transform.rotation);

        yield return new WaitForSeconds(_duration);

        for (int i = 1; i < PlayerCardManager.Instance.CardsInHand.Count; i++)
        {
            var previousCard = card;
            card = PlayerCardManager.Instance.CardsInHand[i];

            card.GetComponent<PlayerCardLogic>().SetAnimationPlacement(newPosition: card.transform.position + _idleV, newRotation: card.transform.rotation);
            previousCard.GetComponent<PlayerCardLogic>().ResetLastPlacement();
            yield return new WaitForSeconds(_duration);
        }
        
        card = PlayerCardManager.Instance.CardsInHand[^1];
        card.GetComponent<PlayerCardLogic>().ResetLastPlacement();
        WaveAnimationActive = false;
    }

    public void ActivatePlacementPreviewAnimation()
    {
        enabled = false;

        foreach (var card in PlayerCardManager.Instance.SelectedCards)
        {
            var pos = _newPositions[PlayerCardManager.Instance.SelectedCards.IndexOf(card)];
            card.GetComponent<PlayerCardLogic>().SetAnimationPlacement(newPosition: pos.position, newRotation: pos.rotation);
        }
    }

    public void DeactivatePlacementPreviewAnimation()
    {
        foreach (var card in PlayerCardManager.Instance.SelectedCards)
        {
            card.GetComponent<PlayerCardLogic>().ResetLastPlacement();
        }

        enabled = true;
    }

}
