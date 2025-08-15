using System.Collections;
using UnityEngine;

public class EnemyTease : MonoBehaviour
{
    public static EnemyTease instance;

    [SerializeField] GameObject teasePanel;

    [SerializeField] float teaseTime;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void Tease()
    {
        StartCoroutine("showWindow");
    }

    IEnumerator showWindow()
    {
        teasePanel.SetActive(true);
        yield return new WaitForSeconds(teaseTime);
        teasePanel.SetActive(false);
    }
}
