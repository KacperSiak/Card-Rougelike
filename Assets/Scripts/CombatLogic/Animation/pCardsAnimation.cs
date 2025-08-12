using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pCardsAnimation : MonoBehaviour
{
    [SerializeField] private float _interval = 3;
    [SerializeField] private float _duration = 0.1f;
    private float _currentTime = 0;
    private Vector3 _idleV = new Vector3(0, 0.0001f, 0f); // Idle animation vector

    public static pCardsAnimation Instance { get; private set; }

    [SerializeField] private List<Transform> _newPositions;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (_currentTime < _interval)
        {
            _currentTime += Time.deltaTime;
            return;
        }

        StartCoroutine(IdleAnimation());
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
        
        card = PlayerCardManager.Instance.CardsInHand[PlayerCardManager.Instance.CardsInHand.Count - 1];
        card.GetComponent<PlayerCardLogic>().ResetLastPlacement();
        _currentTime = 0;
    }

    public void ActivatePlacementPreviewAnimation()
    {
        enabled = false;
        _currentTime = 0;

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
