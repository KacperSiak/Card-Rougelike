using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//here changes
public class ObjPoolManager : MonoBehaviour
{
    public static ObjPoolManager Instance { get; private set; }

    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _start;
    [SerializeField] private Transform _end;
    [SerializeField] float _duration = 0.2f;

    private List<GameObject> _objPool = new();

    public int QueueSize { get; private set; } = 0;
    public bool IsDeckReady { get; private set; } = false;


    private void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);

        RoundManager.OnCombatStart += CreateObjPool;

    }

    //setup could be different then 
    private void CreateObjPool()
    {
        QueueSize = RoundManager.Instance.PlayerCards.Count;

        for (int i = 0; i < QueueSize; i++)
        {
            GameObject obj = Instantiate(_prefab);
            obj.SetActive(false);
            obj.transform.SetPositionAndRotation(_start.position, Quaternion.identity);
            _objPool.Add(obj);
            obj.transform.localScale *= 1.7f;
        }

        StartCoroutine(StartDeckAnimation());
    }

    private IEnumerator StartDeckAnimation()
    {
        Vector3 endPos = _end.transform.position;

        foreach (GameObject obj in _objPool)
        {
            obj.transform.GetPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
            obj.SetActive(true);
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                obj.transform.SetPositionAndRotation(Vector3.Lerp(startPos, endPos, elapsed / _duration), Quaternion.Slerp(startRot, _end.transform.rotation, elapsed / _duration));
                yield return null;
            }

            endPos += new Vector3(0, 0.001f, 0);
        }

        IsDeckReady = true;
    }

    public GameObject GetCardFromPool()
    {
        GameObject card = _objPool[_objPool.Count - 1];
        _objPool.Remove(card);
        return card;
    }

    public void ReturnCard(GameObject card)
    { 
        _objPool.Add(card);
    }
    
}
