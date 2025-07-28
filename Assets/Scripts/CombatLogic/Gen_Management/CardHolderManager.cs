using System.Collections.Generic;
using UnityEngine;

public class CardHolderManager : MonoBehaviour
{
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _defaultMaterial;
    private bool _isOccupied = false;

    public Validator ValidatorType { get; private set; } = null;

    public int EnemyComboIndex { get; private set; } = -1;
    public  List<PlayerCardLogic> PCardScripts { get; private set; }

    public int ValidatedPlayerCardsPower { get; private set; } = 0;

    private void OnEnable()
    {
        RoundManager.OnCombatEnd += OnCombatEnd;
    }


    private void OnDisable()
    {
        RoundManager.OnCombatEnd -= OnCombatEnd;
    }

    public void SetProperties(Validator validator, int enemyIndex)
    {
        ValidatorType = validator;
        EnemyComboIndex = enemyIndex;
        PCardScripts = new List<PlayerCardLogic>();
    }

    private void OnMouseEnter()
    {
        if (!_isOccupied && PlayerCardManager.Instance.SelectedCards.Count != 0 && PlayerCardManager.Instance.SelectedCards.Count != 2)
            gameObject.GetComponent<MeshRenderer>().material = _highlightMaterial;
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = _defaultMaterial;
    }

    private void OnMouseDown()
    {
        if (PlayerCardManager.Instance.SelectedCards.Count == 2 || _isOccupied)
        {
            return;
        }

        foreach (GameObject card in PlayerCardManager.Instance.SelectedCards)
        {
            PCardScripts.Add(card.GetComponent<PlayerCardLogic>());
        }

        ValidatorType.Validate(this);
    }

    public void OnValidated()
    {
        _isOccupied = true;
        int position = -1;

        foreach (GameObject card in PlayerCardManager.Instance.SelectedCards)
        {
            card.transform.position = new Vector3(transform.position.x + position, transform.position.y, transform.position.z);
            position += 1;
            card.GetComponent<PlayerCardLogic>().IsUsed = true;
        }

        ValidatedPlayerCardsPower = PlayerCardManager.Instance.SelectedCardsPower;
        PlayerCardManager.Instance.ClearSelectedCards();

    }

    public void OnValidationFailed()
    {
        PCardScripts.Clear();
    }

    public void ResetCardHolderPlayerCards()
    { 
        _isOccupied = false;
        ValidatedPlayerCardsPower = 0;

        foreach(var card in PCardScripts)
        {
            card.gameObject.SetActive(false);
            card.ResetCard();
        }

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

}
