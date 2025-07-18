using System.Collections.Generic;
using UnityEngine;

public class CardHolderManager : MonoBehaviour
{
    [SerializeField] private Material _highlightMaterial;
    [SerializeField] private Material _defaultMaterial;
    [SerializeField] private Validator _validator;
    private bool _isOccupied = false;

    public ComboTypes ComboType { get; private set; }
    public int EnemyCombo { get; private set; }
    public  List<PlayerCardLogic> PCardScript { get; private set; }

    public int ValidatedPlayerCardsPower { get; private set; }

    //Maybe I will removw this list, not sure if I need it fpr future logic
    public List<GameObject> PlayerCards { get; private set; }

    public void SetProperties(ComboTypes comboType, int enemyIndex)
    {
        this.ComboType = comboType;
        EnemyCombo = enemyIndex;
        PCardScript = new List<PlayerCardLogic>();
        PlayerCards = new List<GameObject>();
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
            PlayerCards.Add(card);
            PCardScript.Add(card.GetComponent<PlayerCardLogic>());
            Debug.Log("playerCards count: " + PlayerCards.Count);
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
        PlayerCards.Clear();
        PCardScript.Clear();
        Debug.Log("playerCards count: " + PlayerCards.Count);
        Debug.Log("pCards count: " + PCardScript.Count);
    }

}
