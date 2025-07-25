using UnityEngine;

public class CardInventoryUI : MonoBehaviour
{
	[SerializeField] private GameObject _slotPrefab;
	[SerializeField] private Transform _slotParent;
	
	public void AddCard(ConsumablesData card)
	{
		var slot = Instantiate(_slotPrefab, _slotParent);
		slot.GetComponent<CardSlotUI>().Setup(card);
	}
}
