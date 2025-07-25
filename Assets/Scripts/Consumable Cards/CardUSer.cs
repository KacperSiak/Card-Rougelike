using UnityEngine;
using UnityEngine.UI;


public class CardUser : MonoBehaviour
{
    [SerializeField] private ConsumablesData _card;
    [SerializeField] private Button _useButton;
    public PlayerOverlay player;

    void Start()
    {
        _useButton.onClick.AddListener(OnButtonClicked);
    }

    void OnButtonClicked()
    {
        var ctx = new ConsumableContext
        {
            player = player
        };
        foreach (var effect in _card.cardEffects)
        {
            effect.ApplyEffect(ctx);
        }
    }
}
