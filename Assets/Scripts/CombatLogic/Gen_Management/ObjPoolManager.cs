using System.Collections.Generic;
using UnityEngine;

public class ObjPoolManager : MonoBehaviour
{
    public static ObjPoolManager Instance { get; private set; }

    [SerializeField] private GameObject _prefab;

    private Queue<GameObject> _objPool = new();

    public int QueueSize { get; private set; } = 0;
    

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

        RoundManager.OnCombatStart += CreateObjPool;

    }

    //setup will be different then 
    private void CreateObjPool()
    {
        QueueSize = RoundManager.Instance.PlayerCards.Count;

        for (int i = 0; i < QueueSize; i++)
        {
            GameObject obj = Instantiate(_prefab);
            //CardsSOTest cardData = RoundManager.Instance.PlayerCards[i];
            obj.SetActive(false);
            _objPool.Enqueue(obj);
            //CardSetupManager.Instance.SetupCardPhysics(obj);
            //CardSetupManager.Instance.SetupCardVisual(obj.GetComponent<MeshRenderer>(), cardData.column, cardData.row);
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
