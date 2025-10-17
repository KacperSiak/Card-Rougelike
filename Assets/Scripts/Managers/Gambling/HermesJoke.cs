using UnityEngine;

[CreateAssetMenu(fileName = "HermesJoke", menuName = "Gambling/HermesJoke")]
public class HermesJoke : ScriptableObject
{
    public string jokeID;
    public Texture2D atlas;
    public int rows;
    public int cols;
    public int row;
    public int col;
}
