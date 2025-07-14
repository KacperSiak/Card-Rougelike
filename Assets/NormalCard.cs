using UnityEngine;

[CreateAssetMenu(fileName = "NormalCard", menuName = "Scriptable Objects/NormalCard")]
public class NormalCard : ScriptableObject
{
    public string cardName;
    public int CardValue;
    public Sprite face;
}
