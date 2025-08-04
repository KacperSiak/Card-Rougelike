using UnityEngine;

public enum Suits
{
    Hearts,
    Diamonds,
    Clubs,
    Spades,
    Joker
}

[CreateAssetMenu(fileName = "CardsSOTest", menuName = "Scriptable Objects/CardsSOTest")]

public class CardsSOTest : ScriptableObject
{
    public int power;
    public Sprite graphics;
    public Suits suit;
}
