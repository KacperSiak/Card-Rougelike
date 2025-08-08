using UnityEngine;
using UnityEngine.UI;

public class ChestOpen : MonoBehaviour
{
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponentInChildren<Animator>();
        
    }

    private void OnMouseDown()
    {
        _anim.SetTrigger("ChestOpen");
        GamblingManager.instance.Roll();
    }
}
