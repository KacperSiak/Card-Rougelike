using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HorseOpen : MonoBehaviour
{
    [SerializeField] private Animator chestAnimator, decreaseAnimator;
    private GamblingSceneManager manager;
    private Animator anim;

    float time;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        manager = GamblingSceneManager.Instance;
    }

    private void OnMouseDown() => Open();

    private void Open()
    {
        if (!manager.canRoll) return;

        manager.canRoll = false;
        anim.SetTrigger("HorseOpen");
        chestAnimator.SetTrigger("RollChest");
        decreaseAnimator.SetTrigger("Decrease");
    }
}
