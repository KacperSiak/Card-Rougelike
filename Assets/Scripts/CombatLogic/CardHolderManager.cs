using System.Collections.Generic;
using UnityEngine;

public class CardHolderManager : MonoBehaviour
{
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Validator _validator;
    private bool _isOccupied = false;
    public IValidate ValidatorType { get; private set; } = null;

    public int EnemyComboIndex { get; private set; } = -1;
    public  List<PlayerCardLogic> PCardScript { get; private set; }

    public int ValidatedPlayerCardsPower { get; private set; } = 0;

    public void SetProperties(IValidate validationType, int enemyIndex)
    {
        ValidatorType = validationType;
        EnemyComboIndex = enemyIndex;
        PCardScript = new List<PlayerCardLogic>();
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
            PCardScript.Add(card.GetComponent<PlayerCardLogic>());
            Debug.Log("playerCardsScript count: " + PCardScript.Count);
        }

        _validator.ValidateCards(this);
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
        PCardScript.Clear();
        Debug.Log("pCards count: " + PCardScript.Count);
    }

    public void ResetCardHolderPlayerCards()
    { 
        _isOccupied = false;
        ValidatedPlayerCardsPower = 0;

        foreach(var card in PCardScript)
        {
            card.gameObject.SetActive(false);
            card.ResetCardPosition();
        }

        PCardScript.Clear();
    }

    public void ResetCardHolderEnemyCards()
    {
        ValidatorType = null;
        EnemyComboIndex = -1;
    }

}
