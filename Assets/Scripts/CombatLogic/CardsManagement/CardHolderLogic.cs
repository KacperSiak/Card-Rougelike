using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CardHolderLogic : MonoBehaviour
{
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private PlacementSFX _placementSFX;
    [field: SerializeField] public CardHolderAnimation CardHolderAnimation { get; private set; }
    [SerializeField] private PlayerCardsAnimation _playerCardsAnimation;
    [SerializeField] private float _duration;
    private bool _isOccupied = false;
    private bool _isInspected = false;

    public Validator ValidatorType { get; private set; } = null;

    public int EnemyComboIndex { get; private set; } = -1;
    public List<PlayerCardLogic> PCardScripts { get; private set; } = new();

    public int ValidatedPlayerCardsPower { get; private set; } = 0;

    public static UnityEvent OnBlackPlay = new UnityEvent();
    public static UnityEvent OnRedPlay = new UnityEvent();

    private void OnEnable()
    {
        RoundManager.OnCombatEnd += OnCombatEnd;
        RoundManager.OnRoundEnd += OnValidationFailed;
    }


    private void OnDisable()
    {
        RoundManager.OnCombatEnd -= OnCombatEnd;
        RoundManager.OnRoundEnd += OnValidationFailed;
    }

    public void SetProperties(Validator validator, int enemyIndex)
    {
        ValidatorType = validator;
        EnemyComboIndex = enemyIndex;
        PCardScripts.Clear();
    }

    private void OnMouseEnter()
    {
        if (DiscardManager.Instance.InUI) return;

        if (!_isOccupied && PlayerCardManager.Instance.SelectedCards.Count != 0 && PlayerCardManager.Instance.SelectedCards.Count != 2)
        {
            gameObject.GetComponent<MeshRenderer>().material = _highlightMaterial;
            _isInspected = true;
        }

    }

    private void OnMouseExit()
    {
        if (_isInspected)
        {
            gameObject.GetComponent<MeshRenderer>().material = _defaultMaterial;
            _isInspected = false;
        }

    }

    private void OnMouseDown()
    {
        if (PlayerCardManager.Instance.SelectedCards.Count == 2 || _isOccupied || DiscardManager.Instance.InUI) return;

        foreach (GameObject card in PlayerCardManager.Instance.SelectedCards)
        {
            PCardScripts.Add(card.GetComponent<PlayerCardLogic>());
        }

        if (PCardScripts.Count > 0)
        {
            CardHolderAnimation.DisableAnimations();
            ValidatorType.Validate(this);
        }
    }

    public void Attack()
    {
        _playerCardsAnimation.Attack(EnemyCardManager.Instance.cardObjects[EnemyComboIndex], PCardScripts.ConvertAll(card => card.transform).ToArray(), isPlayer: true, key: EnemyComboIndex);
    }

    public void OnValidated()
    {
        CheckColors(PlayerCardManager.Instance.SelectedCards);
        ValidatedPlayerCardsPower = PlayerCardManager.Instance.SelectedCardsPower;
        PlayerCardManager.Instance.ClearSelectedCards();
        StartCoroutine(PlaceCards());
    }

    private IEnumerator PlaceCards()
    {
        CardHolderAnimation mainHand = PlayerCardManager.Instance.GetMainHand().GetComponent<CardHolderAnimation>();
        mainHand.DisableAnimations();
        _isOccupied = true;

        Vector3 _finalSpawnPosition = (PCardScripts.Count == 1) ? transform.position : transform.position + new Vector3(-0.05f, 0.001f, 0);

        foreach (PlayerCardLogic card in PCardScripts)
        {
            float elapsed = 0f;

            Vector3 scale = card.transform.localScale;
            card.transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);

            _placementSFX.Play();
            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / 0.5f);
                card.transform.SetPositionAndRotation(Vector3.Lerp(startPos, _finalSpawnPosition, t), Quaternion.Slerp(startRot, Quaternion.Euler(-90f, 0f, 0f), t));
                card.transform.localScale = Vector3.Lerp(scale, scale * 1.3f, t);
                yield return null;
            }

            card.transform.SetParent(transform);
            card.IsUsed = true;

            _finalSpawnPosition += new Vector3(0.05f, 0.001f, 0);
        }

        CardHolderAnimation.EnableAnimations(isPHand: false);
        mainHand.EnableAnimations(isPHand: true);
    }

    public void OnValidationFailed()
    {
        PCardScripts.Clear();
    }

    public void ResetCardHolderPlayerCards()
    {
        _isOccupied = false;
        ValidatedPlayerCardsPower = 0;
        CardHolderAnimation.DisableAnimations();

        _playerCardsAnimation.RemoveCard(PCardScripts.ConvertAll(card => card.transform).ToArray());

        PCardScripts.Clear();
    }

    public void ResetCardHolderEnemyCards()
    {
        ValidatorType = null;
        EnemyComboIndex = -1;
    }

    private void OnCombatEnd()
    {
        ResetCardHolderEnemyCards();
        ResetCardHolderPlayerCards();
    }

    private void CheckColors(List<GameObject> cardList)
    {
        foreach (var card in cardList)
        { 
            var cardProps = card.GetComponent<PlayerCardLogic>();

            if (cardProps.Card.color == CardColor.None)
            {
                OnRedPlay?.Invoke();
                OnBlackPlay?.Invoke();
                continue;
            }
            
            if (cardProps.Card.color == CardColor.Red)
            {
                OnRedPlay?.Invoke();
            }
            else if (cardProps.Card.color == CardColor.Black)
            {
                OnBlackPlay?.Invoke();
            }
        }
    }

}
