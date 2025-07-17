using UnityEngine;
using UnityEngine.UI;


public class CardUSer : MonoBehaviour
{
    public ConsumablesData card;
    public Button button;
    public PlayerOverlay player;

    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClicked()
    {
        var ctx = new ConsumableContext
        {
            player = player
        };
        foreach (var effect in card.cardEffects)
        {
            effect.ApplyEffect(ctx);
        }
    }
}
