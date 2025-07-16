using UnityEngine;

public class PlayerCardLogic : MonoBehaviour
{
    private Vector3 defaultPosition;
    private bool isSelected = false;
    public bool isUsed = false; 
    public CardsSOTest Card { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            if (DamageManager.Instance.pCards.Count == 3)
                return; // Prevent selecting more than 3 cards

            transform.position += new Vector3(0, 0.1f, 0);
            DamageManager.Instance.pCards.Add(gameObject);
            isSelected = true;
            Debug.Log("pCards count: " + DamageManager.Instance.pCards.Count);
        }

        else if(!isUsed)
        {
            transform.position = defaultPosition; 
            DamageManager.Instance.pCards.Remove(gameObject);
            isSelected = false;
            Debug.Log("pCards count: " + DamageManager.Instance.pCards.Count);
        }

    }
}
