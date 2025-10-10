using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : PersistenSingleton<PlayerManager>
{
    public int gold;

    private int _currentPlayerHP = 50; //temp 
    private int _maxPlayerHP = 50;

    public int availableDiscards = 3;
    public int handsize = 8;

    public static UnityEvent OnHpChange = new UnityEvent();
    public int PlayerHP
    {
        get => _currentPlayerHP;
        set
        {
            _currentPlayerHP = value;
            // Later here can be like event activation to check if we need to finish the whole game or not
        }
    }

    public int MaxHP
    {
        get => _maxPlayerHP;
        set
        {
            _maxPlayerHP = value;
        }
    }

    protected override void OnSingletonAwake() { }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public void GoldChange(int amount, bool add)
    {
        if(add) gold += amount;
        else gold -= amount;
        Debug.Log("Current gold: " + gold);
    }

    public void HealthChange(int amount, bool add)
    {
        if (add)
        {
            if((PlayerHP += amount) > _maxPlayerHP)
            {
                _currentPlayerHP = _maxPlayerHP;
            }
            else
            {
                _currentPlayerHP += amount;
            }

        }
        else _currentPlayerHP -= amount;
        OnHpChange?.Invoke();
    }

    public void MaxHPCHange(int amount)
    {
        int percentege = Mathf.RoundToInt((float)_currentPlayerHP / _maxPlayerHP * 100f);
        _maxPlayerHP = amount;
        _currentPlayerHP = amount * percentege;
        //show it on screen
    }
}
