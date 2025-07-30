using UnityEngine;

//here
public class PlayerCardLogic : MonoBehaviour
{
    private Vector3 defaultPosition;
    private Vector3 defaultSize;
    private bool isSelected = false;

    public bool IsUsed { get; set; } = false;

    public CardsSOTest Card { get; private set; }

    public void SaveProperties()
    {
        defaultPosition = transform.localPosition;
        defaultSize = transform.localScale;
    }

    public void SetCardProperties(CardsSOTest card)
    {
        Card = card;
    }

    private void OnMouseEnter()
    {
        if(!isSelected)
        transform.localPosition += new Vector3(0, 0.1f, -0.1f); // Move the card up slightly when hovered
    }

    private void OnMouseExit()
    {
        if(!isSelected)
        transform.localPosition = defaultPosition; // Reset position when not hovered
    }

    private void OnMouseDown()
    {
        if (!isSelected)
        {
            if (PlayerCardManager.Instance.SelectedCards.Count == 3)
                return; // Prevent selecting more than 3 cards

            transform.localPosition = defaultPosition + new Vector3(0, 0.2f, -0.1f);
            PlayerCardManager.Instance.AddSelectedCards(gameObject);
            isSelected = true;
        }

        else if(!IsUsed)
        {
            transform.localPosition = defaultPosition;
            PlayerCardManager.Instance.RemoveSelectedCards(gameObject);
            isSelected = false;
        }

    }

    public void ResetCard()
    {
        transform.SetParent(null);
        transform.position = Vector3.zero;
        transform.localScale = defaultSize;
        isSelected = false;
        IsUsed = false;
        Card = null; 
    }
}
