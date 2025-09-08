using UnityEngine;

public class KnifeAndPaper : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    private bool isClose;
    private bool debounce;

    private GamblingSceneManager manager;

    private float timer;

    private void Awake()
    {
        isClose = false;
        debounce = false;
    }

    private void Start()
    {
        manager = GamblingSceneManager.Instance;
    }

    private void Update()
    {
        if (debounce)
        {
            timer += Time.deltaTime;
            if(timer >= 2)
            {
                debounce = false;
                timer = 0;
            }
        }
    }

    private void OnMouseDown()
    {
        if (debounce) return;

        if (!isClose)
        {
            anim.SetTrigger("ShowInfo");
            isClose = true;
        }
        else
        {
            anim.SetTrigger("HideInfo");
            isClose = false;
        }

        debounce = true;
    }

    public void DisableRolling() => manager.canRoll = false;
    public void EnableRolling() => manager.canRoll = true;
}
