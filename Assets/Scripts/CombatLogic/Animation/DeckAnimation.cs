using System.Collections;
using UnityEngine;

public class DeckAnimation : PersistenSingleton<DeckAnimation>
{
    [SerializeField] private Transform _endPlayer;
    [SerializeField] private Transform _endEnemy;

    private int _enemySize;
    private int _playerSize;

    [SerializeField] float _duration = 0.2f;
    public bool IsDeckReady { get; private set; } = false;

    public void StartDeckAnimation()
    { 
        _playerSize = ObjPoolManager.Instance.PlayerPoolSize;
        _enemySize = ObjPoolManager.Instance.EnemyPoolSize;
        StartCoroutine(PlayDeckAnimation());
    }

    private IEnumerator PlayDeckAnimation()
    {
        Vector3 endPosPlayer = _endPlayer.transform.position;
        Vector3 endPosEnemy = _endEnemy.transform.position;
        GameObject obj;

        for (int i = 0; i < _playerSize; i++)
        {
            obj = ObjPoolManager.Instance._playerPool[i];
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

            if (i < _enemySize)
            {
                obj = ObjPoolManager.Instance._enemyPool[i];
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
}
