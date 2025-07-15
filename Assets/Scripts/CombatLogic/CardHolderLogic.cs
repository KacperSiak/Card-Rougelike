using System;
using System.Collections.Generic;
using UnityEngine;

public class CardHolderLogic : MonoBehaviour
{
    public int pPower { get; private set; }
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    private bool isOccupied = false;
    private ComboTypes comboType;
    //private List<GameObject> combination;

    public void SetProperties(ComboTypes comboType)
    { 
        this.comboType = comboType;
    }

    private void OnMouseEnter()
    {
        if (!isOccupied && DamageManager.Instance.pCards.Count != 0 && DamageManager.Instance.pCards.Count != 2)
            gameObject.GetComponent<MeshRenderer>().material = highlightMaterial;
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<MeshRenderer>().material = defaultMaterial;
    }

    private void OnMouseDown()
    {
        if (DamageManager.Instance.pCards.Count == 2 || isOccupied)
        {
            return;
        }

        switch(comboType)
            {
            case ComboTypes.Clean:
                ValidateClean();
                break;
            case ComboTypes.Dirty:
                ValidateDirty();
                break;
            case ComboTypes.Single:
                ValidateSingle();
                break;
            }

    }

    //TO DO: Refactor this method, validating should be for all three cards and need to prevent chaotic selection

    private void ValidateSingle()
    {
        bool isValid = true;

        if (DamageManager.Instance.pCards.Count > 1)
        {
            isValid = DamageManager.Instance.pCards[0].GetComponent<PlayerCardLogic>().Card.suit != DamageManager.Instance.pCards[1].GetComponent<PlayerCardLogic>().Card.suit;
        }
        else if (DamageManager.Instance.pCards[0].GetComponent<PlayerCardLogic>().Card.power != DamageManager.Instance.pCards[1].GetComponent<PlayerCardLogic>().Card.power)
        {
            isValid = false;
        }

        if (!isValid) return;

        ReactOnValidated();

    }

    private void ValidateDirty()
    {
        bool isValid = true;

        if (DamageManager.Instance.pCards.Count == 1) 
            isValid = false;

        if (!isValid) return;

        ReactOnValidated();
    }

    private void ValidateClean()
    {
        bool isValid = true;

        if (DamageManager.Instance.pCards.Count > 1)
        {
            isValid = DamageManager.Instance.pCards[0].GetComponent<PlayerCardLogic>().Card.suit == DamageManager.Instance.pCards[1].GetComponent<PlayerCardLogic>().Card.suit;
        }



        if (!isValid) return;

        ReactOnValidated();
    }

    private void ReactOnValidated()
    {
        isOccupied = true;
        int position = -1;

        foreach (GameObject card in DamageManager.Instance.pCards)
        {
            pPower += card.GetComponent<PlayerCardLogic>().Card.power;
            card.transform.position = new Vector3(transform.position.x + position, transform.position.y, transform.position.z);
            position += 1;
            card.GetComponent<PlayerCardLogic>().enabled = false; 
        }

        DamageManager.Instance.pCards.Clear();
    }

}
