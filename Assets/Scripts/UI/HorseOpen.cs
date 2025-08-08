using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HorseOpen : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    private Animator anim;
    private float _delaytime = 1f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        _openButton.onClick.AddListener(Open);
    }

    private void Open()
    {
        anim.SetTrigger("Open");
        StartCoroutine(EnableChestButton());
    }

    IEnumerator EnableChestButton()
    {
        yield return new WaitForSeconds(_delaytime);

    }
}
