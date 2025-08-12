using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine;

public class NewRunDialogue : MonoBehaviour
{
    public static NewRunDialogue instance;

    DialogueManager dialogueManager;

    [SerializeField] TextAsset inkFile;

    private Dictionary<string, int> countOfDefeats;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        countOfDefeats = new Dictionary<string, int>();

        dialogueManager = DialogueManager.GetInstance();
        NewGameAssign();
    }

    // has to be called at the beggining of 'New Game'
    void NewGameAssign()
    {
        countOfDefeats.Add("Minotaur",0);
        countOfDefeats.Add("Arachne",0);
        countOfDefeats.Add("Griffon",0);
        countOfDefeats.Add("Satyr",0);
        countOfDefeats.Add("Athena",0);
    }

    // call this when starting new Run
    public void DialogueByDefeat(string defeatedBy)
    {
        int whichTimeDefeated = countOfDefeats[defeatedBy];
        whichTimeDefeated++;
        countOfDefeats[defeatedBy] = whichTimeDefeated;

        dialogueManager.EnterDialogueMode(inkFile, $"{defeatedBy}.died{whichTimeDefeated}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            DialogueByDefeat("Minotaur");
        }
    }
}
