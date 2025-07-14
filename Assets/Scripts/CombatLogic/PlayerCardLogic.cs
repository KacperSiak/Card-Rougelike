using UnityEngine;

public class PlayerCardLogic : MonoBehaviour
{
    private Vector3 defaultPosition;
    private bool isSelected = false;
    public int PlayerPower { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultPosition = transform.position;
        Debug.Log(PlayerPower);
    }

    public void SetCardProperties(int power)
    {
        PlayerPower = power;
    }

    private void OnMouseEnter()
    {
        if(!isSelected)
        transform.position += new Vector3(0, 0.3f, 0); // Move the card up slightly when hovered
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
            isSelected = true; 
            transform.position = new Vector3(0, 2, 0); // Tems until i create responsive places near enemy cards
        }

    }
}
