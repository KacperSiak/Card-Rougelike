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
    private List<PlayerCardLogic> playerCards = new List<PlayerCardLogic>();
    //private List<GameObject> combination; in case we want to store the combination of enemy's cards, to block certain player reactions(like place card if its power lower than enemy's combo)

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

        foreach (GameObject card in DamageManager.Instance.pCards)
        {
            playerCards.Add(card.GetComponent<PlayerCardLogic>());
            Debug.Log("playerCards count: " + playerCards.Count);
        }

        switch (comboType)
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

    private void ValidateSingle()
    {
        bool isValid = true;

        if (DamageManager.Instance.pCards.Count > 1)
        {
            isValid = playerCards[0].Card.suit != playerCards[playerCards.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < playerCards.Count - 2; i++)
            {
                if (!isValid)
                {
                    playerCards.Clear();
                    Debug.Log("playerCards count: " + playerCards.Count);
                    Debug.Log("valid or not: " + isValid);
                    return;
                }

                isValid = playerCards[i].Card.suit != playerCards[i + 1].Card.suit;
                isValid = playerCards[i].Card.power == playerCards[i + 1].Card.power;
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
        ReactOnValidated();

    }

    private void ValidateDirty()
    {
        bool isValid = true;

        if (DamageManager.Instance.pCards.Count == 1)
        {
            isValid = false;
            playerCards.Clear();
            Debug.Log("playerCards count: " + playerCards.Count);
            return;
        }

        isValid = playerCards[0].Card.suit == playerCards[playerCards.Count - 1].Card.suit;
        Debug.Log("valid or not: " + isValid);

        for (int i = 0; i < playerCards.Count - 2; i++)
        {
            if (!isValid) break;

            isValid = playerCards[i].Card.suit == playerCards[i + 1].Card.suit;
            isValid = playerCards[i].Card.power + 1 == playerCards[i + 1].Card.power;
            Debug.Log("valid or not: " + isValid);
        }

        Debug.Log("valid or not: " + isValid);

        if (!isValid)
        {
            isValid = playerCards[0].Card.suit != playerCards[playerCards.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < playerCards.Count - 2; i++)
            {
                if (!isValid)
                {
                    playerCards.Clear();
                    Debug.Log("valid or not: " + isValid);
                    return;
                }

                isValid = playerCards[i].Card.suit != playerCards[i + 1].Card.suit;
                isValid = playerCards[i].Card.power == playerCards[i + 1].Card.power;
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
        ReactOnValidated();
    }

    private void ValidateClean()
    {
        bool isValid = true;

        if (DamageManager.Instance.pCards.Count > 1)
        {
            isValid = playerCards[0].Card.suit == playerCards[playerCards.Count - 1].Card.suit;
            Debug.Log("valid or not: " + isValid);

            for (int i = 0; i < playerCards.Count - 2; i++)
            {
                if (!isValid)
                {
                    playerCards.Clear();
                    Debug.Log("valid or not: " + isValid);
                    return;
                } 

                isValid = playerCards[i].Card.suit == playerCards[i + 1].Card.suit;
                isValid = playerCards[i].Card.power + 1 == playerCards[i + 1].Card.power;
                Debug.Log("valid or not: " + isValid);
            }
        }

        Debug.Log("valid or not: " + isValid);
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
            card.GetComponent<PlayerCardLogic>().isUsed = true;
        }

        DamageManager.Instance.pCards.Clear();
        playerCards.Clear();
        Debug.Log("playerCards count: " + playerCards.Count);
        Debug.Log("pCards count: " + DamageManager.Instance.pCards.Count);
    }

}
