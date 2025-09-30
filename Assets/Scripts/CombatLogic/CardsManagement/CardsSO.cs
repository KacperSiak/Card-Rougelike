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

[CreateAssetMenu(fileName = "CardsSO", menuName = "Scriptable Objects/CardsSO")]

//will be main cardSO script
public class CardsSO : ScriptableObject
{
    public int power;
    public int column;
    public int row;
    public Suits suit;
    public CardColor color;
}
