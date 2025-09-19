using UnityEngine;

public enum Suits
{
    Clubs,
    Spades,
    Hearts,
    Diamonds,
    Joker
}

public enum CardColor
{
    Red,
    Black,
    None
}

[CreateAssetMenu(fileName = "CardsSOTest", menuName = "Scriptable Objects/CardsSOTest")]

//will be main cardSO script
public class CardsSOTest : ScriptableObject
{
    public int power;
    public int column;
    public int row;
    public Suits suit;
    public CardColor color;
}
