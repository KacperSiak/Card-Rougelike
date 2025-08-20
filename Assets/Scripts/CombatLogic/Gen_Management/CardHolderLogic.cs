using System.Collections.Generic;
using UnityEngine;

public class CardHolderLogic : MonoBehaviour
{
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _defaultMaterial;
    private bool _isOccupied = false;
    private bool _isInspected = false;

    public Validator ValidatorType { get; private set; } = null;

    public int EnemyComboIndex { get; private set; } = -1;
    public  List<PlayerCardLogic> PCardScripts { get; private set; }

    public int ValidatedPlayerCardsPower { get; private set; } = 0;

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
        PCardScripts = new List<PlayerCardLogic>();
    }

    private void OnMouseEnter()
    {
        if (!_isOccupied && PlayerCardManager.Instance.SelectedCards.Count != 0 && PlayerCardManager.Instance.SelectedCards.Count != 2)
        {
            gameObject.GetComponent<MeshRenderer>().material = _highlightMaterial;
            CamMovementManager.Instance.enabled = false;
            CamMovementManager.Instance.ChangeCameraPlacement(targetKey: "W");
            _isInspected = true;
            PlayerCardsAnimation.Instance.ActivatePlacementPreviewAnimation();
        }

    }

    private void OnMouseExit()
    {
        if (_isInspected)
        {
            gameObject.GetComponent<MeshRenderer>().material = _defaultMaterial;
            CamMovementManager.Instance.ChangeCameraPlacement(targetKey: "Main");
            CamMovementManager.Instance.enabled = true;
            _isInspected = false;
            PlayerCardsAnimation.Instance.DeactivatePlacementPreviewAnimation();
        }
        
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

        float position = -0.05f;
        float y = 0.001f;

        foreach (GameObject card in PlayerCardManager.Instance.SelectedCards) 
        {
            card.transform.position = new Vector3(transform.position.x + position, transform.position.y + y, transform.position.z);
            card.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            card.transform.localScale *= 1.2f;
            position += 0.05f;
            y += 0.001f;
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
            ObjPoolManager.Instance.ReturnCard(card.gameObject);
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
