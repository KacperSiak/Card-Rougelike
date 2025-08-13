using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HorseOpen : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    private Animator anim;
    private bool _canOpen;

    void Awake()
    {
        anim = GetComponent<Animator>();
        _openButton.onClick.AddListener(Open);
        _canOpen = true;
    }

    private void Open()
    {
        if (_canOpen) anim.SetTrigger("Open");
        _canOpen = false;
    }

  
}
