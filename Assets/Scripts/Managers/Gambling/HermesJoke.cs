using UnityEngine;

[CreateAssetMenu(fileName = "HermesJoke", menuName = "Gambling/HermesJoke")]
public class HermesJoke : ScriptableObject
{
    [TextArea] public string joke;
    public Sprite jokeIcon;
}
