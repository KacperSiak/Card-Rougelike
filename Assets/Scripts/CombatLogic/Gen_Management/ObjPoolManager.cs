using System.Collections.Generic;
using UnityEngine;

//here changes
public class ObjPoolManager : PersistenSingleton<ObjPoolManager>
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _prefabPlayer;
    [SerializeField] private GameObject _prefabEnemy;

    [Header("AnimationPositions")]
    [SerializeField] private Transform _startPlayer;
    [SerializeField] private Transform _startEnemy;

    public List<GameObject> _enemyPool { get; private set; } = new();
    public List<GameObject> _playerPool { get; private set; } = new();
    private List<GameObject> _usedCards = new();

    public int PlayerPoolSize { get; private set; } = 0;
    public int EnemyPoolSize { get; private set; } = 0;

    private void Start()
    {
        CreateObjPool();
    }

    private void CreateObjPool()
    {
        PlayerPoolSize = RoundManager.Instance.PlayerCards.Count;
        EnemyPoolSize = CombatManager.Instance.GetCurrentDeckSize();

        for (int i = 0; i < PlayerPoolSize; i++)
        {
            CreateCard(_prefabPlayer, _startPlayer, _playerPool);
        }

        for (int i = 0; i < EnemyPoolSize; i++)
        {
            CreateCard(_prefabEnemy, _startEnemy, _enemyPool);
        }

        DeckAnimation.Instance.StartDeckAnimation();
    }

    private void CreateCard(GameObject prefab, Transform start, List<GameObject> pool)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetPositionAndRotation(start.position, Quaternion.identity);
        pool.Add(obj);
        obj.transform.localScale *= 1.7f;
        obj.transform.SetParent(transform, true);
    }

    public GameObject GetCardFromPool(bool isSenderPlayer)
    {
        if (isSenderPlayer)
        {
            GameObject card = _playerPool[^1];
            _playerPool.Remove(card);
            card.transform.SetParent(null, true);
            return card;
        }
        else
        {
            GameObject card = _enemyPool[^1];
            _enemyPool.Remove(card);
            card.transform.SetParent(null, true);
            return card;
        }
    }

    public void ReturnCard(GameObject card)
    { 
        _usedCards.Add(card);
        card.SetActive(false);
        card.transform.SetParent(transform, true);
    }
    
}
