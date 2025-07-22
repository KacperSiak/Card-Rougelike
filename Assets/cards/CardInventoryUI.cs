using UnityEngine;

public class CardInventoryUI : MonoBehaviour
{
	public GameObject slotPrefab;
	public Transform slotParent;
	public PlayerOverlay player;
	
	public void AddCard(ConsumablesData card)
	{
		Debug.Log("Dodabno Kartê: " + card.cardName);
		var slot = Instantiate(slotPrefab, slotParent);
		slot.GetComponent<CardSlotUI>().Setup(card, player);
	}
}
