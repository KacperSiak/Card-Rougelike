using UnityEngine;

[CreateAssetMenu(fileName = "CardVisualData", menuName = "Scriptable Objects/CardVisualData")]
public class CardVisualData : ScriptableObject
{
    public string CardID;
    public string cardName;
    [TextArea] public string description;
    public Sprite icon;
}
