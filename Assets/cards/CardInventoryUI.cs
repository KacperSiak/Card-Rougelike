using UnityEngine;

public class CardInventoryUI : MonoBehaviour
{
	public GameObject slotPrefab;
	public Transform slotParent;
	public PlayerOverlay player;
	
	public void AddCard(ConsumablesData card)
	{
		var slot = Instantiate(slotPrefab, slotParent);
		slot.GetComponent<CardSlotUI>().Setup(card, player);
	}
}
