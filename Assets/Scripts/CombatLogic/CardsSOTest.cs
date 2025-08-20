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

//will be main cardSO script
public class CardsSOTest : ScriptableObject
{
    public int power;
    public int column;
    public int row;
    public Suits suit;
}
