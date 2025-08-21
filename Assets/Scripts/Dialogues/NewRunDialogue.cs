using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine;

public class NewRunDialogue : MonoBehaviour
{
    public static NewRunDialogue instance;

    private DialogueManager dialogueManager;

    [SerializeField] private TextAsset inkFile;

    private Dictionary<string, int> countOfDefeats;

    [SerializeField] string defeatedBy;

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

    // has to be called when save file is loaded first time
    void NewGameAssign()
    {
        countOfDefeats.Add("Minotaur",0);
        countOfDefeats.Add("Arachne",0);
        countOfDefeats.Add("Griffon",0);
        countOfDefeats.Add("Satyr",0);
        countOfDefeats.Add("Athena",0);
    }

    // this is function to be called when dialogue on new run has to show up
    public void DialogueByDefeat(string defeatedBy)
    {
        if (dialogueManager.isDialoguePlaying) return;

        int whichTimeDefeated = countOfDefeats[defeatedBy];
        whichTimeDefeated++;
        countOfDefeats[defeatedBy] = whichTimeDefeated;

        PlayerPrefs.SetInt(defeatedBy, whichTimeDefeated);

        dialogueManager.EnterDialogueMode(inkFile, $"{defeatedBy}.died{whichTimeDefeated}");
    }
}
