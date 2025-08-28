using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Like that for now, but for future extension, and modification, we also need manager for the decks and kinda a game manager, to take care of things like cursor, etc.

    public static PlayerManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }
    
    private int _playerHP = 50; //temp 

    public int PlayerHP
    {
        get => _playerHP;
        set
        {
            _playerHP = value;
            // Later here can be like event activation to check if we need to finish the whole game or not
        }
    }
}
