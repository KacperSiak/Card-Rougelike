using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCardsAnimation : CardsAnimation
{
    [SerializeField] private Vector3[] _idleVectors;
    [field: SerializeField] public int Interval = 3;
    [SerializeField] private float _duration = 0.1f;
    private Vector3 _idleV = new();
    public bool WaveAnimationActive { get; private set; } = false;

    [SerializeField] private List<Transform> _newPositions;

    private void OnEnable()
    {
        PlayerCardManager.OnViewChanged += OnViewChanged;
        OnViewChanged();
    }

    private void OnDisable()
    { 
        PlayerCardManager.OnViewChanged -= OnViewChanged;
    }

    private void OnViewChanged()
    { 
        _idleV = CamMovementManager.Instance.IsFirstSet ? _idleVectors[0] : _idleVectors[1];
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
        card.GetComponent<PlayerCardLogic>().SetAnimationPlacement(newPosition: _idleV, newRotation: card.transform.rotation);

        yield return new WaitForSeconds(_duration);

        for (int i = 1; i < PlayerCardManager.Instance.CardsInHand.Count; i++)
        {
            var previousCard = card;
            card = PlayerCardManager.Instance.CardsInHand[i];

            card.GetComponent<PlayerCardLogic>().SetAnimationPlacement(newPosition: _idleV, newRotation: card.transform.rotation);
            previousCard.GetComponent<PlayerCardLogic>().ResetLastPlacement(_idleV);
            yield return new WaitForSeconds(_duration);
        }
        
        card = PlayerCardManager.Instance.CardsInHand[^1];
        card.GetComponent<PlayerCardLogic>().ResetLastPlacement(_idleV);
        WaveAnimationActive = false;
    }

}
