using UnityEngine;

public class PlayerCardLogic : MonoBehaviour
{
    private Vector3 defaultPosition;
    private bool isSelected = false;
    public CardsSOTest Card { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultPosition = transform.position;
        Debug.Log(Card.power);
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
            transform.position += new Vector3(0, 0.1f, 0);
            DamageManager.Instance.pCards.Add(gameObject);
            isSelected = true;
        }
        else
        {
            transform.position = defaultPosition; 
            DamageManager.Instance.pCards.Remove(gameObject);
            isSelected = false;
        }

    }
}
