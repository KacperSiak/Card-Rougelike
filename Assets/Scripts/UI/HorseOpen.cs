using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HorseOpen : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    private Animator anim;
    private Animator _anim;
    private bool _canOpen;

	private void OnValidate()
	{
		if(!_anim)
            _anim = GetComponent<Animator>();
	}

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
