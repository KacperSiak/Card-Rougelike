using UnityEngine;

public class PlayerCardLogic : MonoBehaviour
{
    private Vector3 defaultPosition;
    private Vector3 defaultSize;
    private bool isSelected = false;

    public bool IsUsed { get; set; } = false;

    public CardsSOTest Card { get; private set; }

    private void OnEnable() 
    {
        RoundManager.OnRoundEnd += MakeReusable;
    }


    private void OnDisable()
    { 
        RoundManager.OnRoundEnd -= MakeReusable;
    }

    public void SaveProperties()
    {
        defaultPosition = transform.position;
        defaultSize = transform.localScale;
    }

    public void SetCardProperties(CardsSOTest card)
    {
        Card = card;
    }

    private void OnMouseEnter()
    {
        if (isSelected) return;
        transform.position += new Vector3(0, 0.1f, -0.1f); // Move the card up slightly when hovered
        GetComponent<SpriteRenderer>().sortingOrder++;
    }

    private void OnMouseExit()
    {
        if (isSelected) return; 
        transform.position = defaultPosition; // Reset position when not hovered
        GetComponent<SpriteRenderer>().sortingOrder--;
    }

    private void OnMouseDown()
    {
        if (!isSelected)
        {
            if (PlayerCardManager.Instance.SelectedCards.Count == 3)
                return; // Prevent selecting more than 3 cards

            transform.position = defaultPosition + new Vector3(0, 0.15f, -0.1f);
            PlayerCardManager.Instance.AddSelectedCards(gameObject);
            isSelected = true;
        }

        else if(!IsUsed)
        {
            transform.position = defaultPosition;
            PlayerCardManager.Instance.RemoveSelectedCards(gameObject);
            isSelected = false;
        }

    }
    public void MakeReusable()
    {
        isSelected = false;
        IsUsed = false;
    }

    public void ResetCard()
    {
        transform.position = Vector3.zero;
        transform.localScale = defaultSize;
        Card = null; 
        MakeReusable();
    }
}
