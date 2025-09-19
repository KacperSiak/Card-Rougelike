using UnityEngine;
using UnityEngine.UI;

public class DialoguePanelClick : MonoBehaviour
{
    private DialogueManager dialogueManager;
    [SerializeField]private Button button;

    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
        button.onClick.AddListener(()=>dialogueManager.InteractionWithDialogueBox());
    } 
}
