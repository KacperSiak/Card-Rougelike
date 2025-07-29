using System.Collections.Generic;
using UnityEngine;

public class ObjPoolManager : MonoBehaviour
{
    public static ObjPoolManager Instance { get; private set; }

    [SerializeField] private GameObject _prefab;

    private Queue<GameObject> _objPool = new();
    private int _queueSize = 0;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

        RoundManager.OnCombatStart += CreateObjPool;

    }

    private void CreateObjPool()
    { 
        _queueSize = RoundManager.Instance.PlayerCards.Count;

        for (int i = 0; i < _queueSize; i++)
        {
            GameObject obj = Instantiate(_prefab);
            obj.SetActive(false);
            _objPool.Enqueue(obj);
        }

    }

    public GameObject GetCardFromPool()
    {
        return _objPool.Dequeue();
    }

    public void ReturnCard(GameObject card)
    { 
        _objPool.Enqueue(card);
    }
    
}
