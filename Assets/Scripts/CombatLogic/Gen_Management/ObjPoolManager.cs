using System.Collections;
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
    [SerializeField] private Transform _endPlayer;
    [SerializeField] private Transform _startEnemy;
    [SerializeField] private Transform _endEnemy;

    [SerializeField] float _duration = 0.2f;

    private List<GameObject> _enemyPool = new();
    private List<GameObject> _playerPool = new();
    private List<GameObject> _usedCards = new();

    public int PlayerPoolSize { get; private set; } = 0;
    public int EnemyPoolSize { get; private set; } = 0;
    public bool IsDeckReady { get; private set; } = false;

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

        StartCoroutine(PlayDeckAnimation());
    }

    private void CreateCard(GameObject prefab, Transform start, List<GameObject> pool)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetPositionAndRotation(start.position, Quaternion.identity);
        pool.Add(obj);
        obj.transform.localScale *= 1.7f;
    }

    private IEnumerator PlayDeckAnimation()
    {
        Vector3 endPosPlayer = _endPlayer.transform.position;
        Vector3 endPosEnemy = _endEnemy.transform.position;
        GameObject obj;

        //think for better later
        for (int i = 0; i < PlayerPoolSize; i++)
        {
            obj = _playerPool[i];
            obj.transform.GetPositionAndRotation(out Vector3 startPosP, out Quaternion startRotP);
            obj.SetActive(true);
            float elapsed = 0f;

            while (elapsed < _duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _duration);
                obj.transform.SetPositionAndRotation(Vector3.Lerp(startPosP, endPosPlayer, t), Quaternion.Slerp(startRotP, _endPlayer.transform.rotation, t));
                yield return null;
            }

            endPosPlayer += new Vector3(0, 0.001f, 0);

            if (i < EnemyPoolSize)
            {
                obj = _enemyPool[i];
                obj.transform.GetPositionAndRotation(out Vector3 startPosE, out Quaternion startRotE);
                obj.SetActive(true);
                elapsed = 0f;

                while (elapsed < _duration)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.SmoothStep(0f, 1f, elapsed / _duration);
                    obj.transform.SetPositionAndRotation(Vector3.Lerp(startPosE, endPosEnemy, t), Quaternion.Slerp(startRotE, _endEnemy.transform.rotation, t));
                    yield return null;
                }
                endPosEnemy += new Vector3(0, 0.001f, 0);
            }
        }

        IsDeckReady = true;
    }

    public GameObject GetCardFromPool(bool isSenderPlayer)
    {
        if (isSenderPlayer)
        {
            GameObject card = _playerPool[^1];
            card.transform.localScale /= 1.7f; //temp
            _playerPool.Remove(card);
            return card;
        }
        else
        {
            GameObject card = _enemyPool[^1];
            _enemyPool.Remove(card);
            return card;
        }
    }

    public void ReturnCard(GameObject card)
    { 
        _usedCards.Add(card);
        card.SetActive(false);
    }
    
}
