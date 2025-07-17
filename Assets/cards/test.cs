using UnityEngine;

public class test : MonoBehaviour
{
    public CardInventoryUI inventory;
    public ConsumablesData cons1;
    public ConsumablesData cons2;
    public ConsumablesData cons3;

    private void Start()
    {
        Debug.Log("Wykonuje siê");
        inventory.AddCard(cons1);
        inventory.AddCard(cons2);
        inventory.AddCard(cons3);
    }
}
