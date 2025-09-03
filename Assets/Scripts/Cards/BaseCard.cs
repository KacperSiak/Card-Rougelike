using UnityEngine;

[CreateAssetMenu(fileName = "BaseCard", menuName = "Scriptable Objects/BaseCard")]
public class BaseCard : ScriptableObject
{
    public string cardName;
    public int CardValue;
    public Sprite icon;
}
