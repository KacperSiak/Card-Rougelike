using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _playerFire;
    [SerializeField] private List<GameObject> _enemyFire;
    [SerializeField] private int _midHealthLevel;
    [SerializeField] private int _criticalHealthLevel;

    private void OnEnable()
    {
        RoundManager.OnHPChanged += SetHealthVisual;
    }

    private void OnDisable()
    {
        RoundManager.OnHPChanged -= SetHealthVisual;
    }

    private void SetHealthVisual()
    {
        SetForPlayer();
        SetForEnemy();
    }

    private void SetForEnemy()
    {
        if (RoundManager.Instance.EnemyHP <= _midHealthLevel)
        {
            for (int i = 0; i < _enemyFire.Count - 1; i++)
            {
                _enemyFire[i].SetActive(false);
            }
        }

        if (RoundManager.Instance.EnemyHP <= _criticalHealthLevel)
        {
            _enemyFire[^1].SetActive(false);
        }
    }

    private void SetForPlayer()
    {
        if (RoundManager.Instance.PlayerHP <= _midHealthLevel)
        {
            for (int i = 0; i < _playerFire.Count - 1; i++)
            {
                _playerFire[i].SetActive(false);
            }
        }

        if (RoundManager.Instance.PlayerHP <= _criticalHealthLevel)
        {
            _playerFire[^1].SetActive(false);
        }
    }
}
