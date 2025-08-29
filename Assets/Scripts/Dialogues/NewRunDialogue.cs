using Ink.Parsed;
using Ink.UnityIntegration;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewRunDialogue : MonoBehaviour
{
    public static NewRunDialogue instance;

    private DialogueManager dialogueManager;
    private OverallDialogueInformation data;

    [SerializeField] private TextAsset defeatInkFile;
    [SerializeField] private TextAsset goodGambleInkFile;
    [SerializeField] private TextAsset badGambleInkFile;
    [SerializeField] private TextAsset firstDeathInkFile;

    [SerializeField] int numberOfDefeatTexts, numberOfGambleTexts;


    [SerializeField] string defeatedBy; // for testing

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    private void Start()
    {
        dialogueManager = DialogueManager.GetInstance();
        data = GetComponent<DialogueData>().OverallDialogueInformation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            dialogueManager.EnterDefeatDialogueMode(defeatedBy);
    }

    // this is function to be called when dialogue on new run has to show up - IT IS CALLED FROM DIALOGUEMANAGER
    public void DefeatDialogue(string defeatedBy)
    {
        if (dialogueManager.isDialoguePlaying) return;

        int deaths = data.numberOfDeaths;
        int goodGamble = data.goodGambleAttempts;
        int badGamble = data.badGambleAttempts;
        int totalGamble = data.allGambleAttempts;

        int bgTries = data.opponentInformation.FirstOrDefault(c => c.opponentName == defeatedBy)?.badGambleDialoguePlayed ?? -1;   // bad gamble tries individual for each opponent
        int ggTries = data.opponentInformation.FirstOrDefault(c => c.opponentName == defeatedBy)?.goodGambleDialoguePlayed ?? -1;  // good gamble tries individual for each opponent

        if (deaths == 1)
            dialogueManager.EnterDialogueMode(firstDeathInkFile);
        else
        {
            int defeatedByCount = data.opponentInformation.FirstOrDefault(c => c.opponentName == defeatedBy)?.numberOfDefeatsByOpponent ?? -1;

            if (defeatedByCount <= numberOfDefeatTexts)
                dialogueManager.EnterDialogueMode(defeatInkFile, $"{defeatedBy}.died{defeatedByCount}");
            else if(defeatedByCount > numberOfDefeatTexts)
            {
                if(goodGamble > badGamble && ggTries <= numberOfGambleTexts)
                {
                    if(totalGamble <= 5)
                    {
                        dialogueManager.EnterDialogueMode(goodGambleInkFile, $"lessThanFive.died{ggTries+1}");
                    }
                    if(totalGamble > 5)
                    {
                        dialogueManager.EnterDialogueMode(goodGambleInkFile, $"moreThanFive.died{ggTries+1}");
                    }
                }
                else if(badGamble > goodGamble && bgTries <= numberOfGambleTexts)
                {
                    if (totalGamble <= 5)
                    {
                        dialogueManager.EnterDialogueMode(badGambleInkFile, $"lessThanFive.died{bgTries + 1}");
                    }
                    if (totalGamble > 5)
                    {
                        dialogueManager.EnterDialogueMode(badGambleInkFile, $"moreThanFive.died{bgTries + 1}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[NewRunDialogue] There is no more dialogue to choose from!");
                }
            }

        }

    }
}
