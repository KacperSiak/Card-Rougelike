using UnityEngine;
using UnityEngine.UI;
public class CardSlotUI : MonoBehaviour
{
    public Image icon;
    public Button useButton;
    public Button discardButton;

    private ConsumablesData cardData;
    private PlayerOverlay player;

    public void Setup(ConsumablesData data, PlayerOverlay p)
    {
        cardData = data;
        player = p;

        icon.sprite = cardData.icon;

        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(UseCard);

        discardButton.onClick.RemoveAllListeners(); 
        discardButton.onClick.AddListener(DiscardCard);
    }

    public void UseCard()
    {
        var ctx = new ConsumableContext
        {
            player = player
        };
        foreach (var effect in cardData.cardEffects)
        {
            effect.ApplyEffect(ctx);
        }

        Destroy(gameObject);
    }

    public void DiscardCard()
    {
        Destroy(gameObject);
    }
}