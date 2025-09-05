using UnityEngine;

public class PlayerManager : PersistenSingleton<PlayerManager>
{
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
