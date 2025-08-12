using UnityEngine;

public class EnemyTease : MonoBehaviour
{
    public static EnemyTease instance;

    private DialogueManager dialogueManager;

    private TextAsset inkJSON;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        dialogueManager = DialogueManager.GetInstance();
    }

    public void Tease()
    {
        dialogueManager.EnterDialogueMode(inkJSON);
    }
}
