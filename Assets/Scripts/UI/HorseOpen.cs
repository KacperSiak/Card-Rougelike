using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HorseOpen : MonoBehaviour
{
    [SerializeField] private Button _openButton;
    [SerializeField] private ChestOpen _chest;
    private Animator anim;
    private bool _canOpen;
    private float time = 0.833f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        _openButton.onClick.AddListener(Open);
        _canOpen = true;
    }

    private void Open()
    {
        if (_canOpen)
        {
            _canOpen = false;
            anim.SetTrigger("HorseOpen");
            StartCoroutine(WaitForHorse(time));
        }
    }

    IEnumerator WaitForHorse(float time)
    {
        yield return new WaitForSeconds(time);
        _chest.ChestOpening();
    }
  
}
