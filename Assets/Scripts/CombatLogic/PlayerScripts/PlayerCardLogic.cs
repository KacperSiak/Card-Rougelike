using UnityEngine;

public class PlayerCardLogic : MonoBehaviour
{
    private Vector3 defaultPosition;
    private bool isSelected = false;

    public bool IsUsed { get; set; } = false;

    public CardsSOTest Card { get; private set; }

    void Start()
    {
        defaultPosition = transform.position;
    }

    public void SetCardProperties(CardsSOTest card)
    {
        Card = card;
    }

    private void OnMouseEnter()
    {
        if(!isSelected)
        transform.position += new Vector3(0, 0.1f, 0); // Move the card up slightly when hovered
    }

    private void OnMouseExit()
    {
        if(!isSelected)
        transform.position = defaultPosition; // Reset position when not hovered
    }

    private void OnMouseDown()
    {
        if (!isSelected)
        {
            if (PlayerCardManager.Instance.SelectedCards.Count == 3)
                return; // Prevent selecting more than 3 cards

            transform.position = defaultPosition + new Vector3(0, 0.2f, 0);
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

    public void ResetCardPosition()
    {
        transform.position = defaultPosition;
    }
}
