using UnityEngine;
using UnityEngine.UI;

public class ChestOpen : MonoBehaviour
{
    private Animator _anim;
    private bool _canOpen;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        _canOpen = true;
    }

    private void OnMouseDown()
    {
        if (_canOpen)
        {
            _canOpen = false;
            _anim.SetTrigger("ChestOpen");
            GamblingManager.instance.Roll();
        }
    }
}
