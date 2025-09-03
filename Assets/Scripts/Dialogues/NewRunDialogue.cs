using Ink.Parsed;
using Ink.UnityIntegration;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewRunDialogue : MonoBehaviour
{
    private DialogueManager dialogueManager;
    private DialogueData data;
    private OverallDialogueInformation info;

    [SerializeField] private TextAsset firstDeathInkFile;
    [SerializeField] private TextAsset defeatInkFile;
    [SerializeField] private TextAsset goodGambleInkFile;
    [SerializeField] private TextAsset badGambleInkFile;

    [SerializeField] int numberOfDefeatTexts, numberOfGambleTexts;


    [SerializeField] string defeatedBy; // for testing


    private void Start()
    {
        dialogueManager = DialogueManager.GetInstance();
        data = GetComponent<DialogueData>();
        info = data.OverallDialogueInformation;
    }

    public void DefeatDialogue(string defeatedBy)
    {
        if (dialogueManager.isDialoguePlaying) return;

        data.IncreaseDeathCount(defeatedBy);

        int deaths = info.numberOfDeaths;
        int goodGamble = info.goodGambleAttempts;
        int badGamble = info.badGambleAttempts;
        int totalGamble = info.allGambleAttempts;

        var opponent = info.opponentInformation.FirstOrDefault(c => c.opponentName == defeatedBy);
        int bgTriesLT5 = opponent?.badGambleDialoguePlayedLessThanFive ?? -1;   // bad gamble tries individual for each opponent - less than 5 total gambling attempts
        int bgTriesMT5 = opponent?.badGambleDialoguePlayedMoreThanFive ?? -1;   // bad gamble tries individual for each opponent - more than 5 total gambling attempts
        int ggTriesLT5 = opponent?.goodGambleDialoguePlayedLessThanFive ?? -1;  // good gamble tries individual for each opponent - less than 5 total gambling attempts
        int ggTriesMT5 = opponent?.goodGambleDialoguePlayedMoreThanFive ?? -1;  // good gamble tries individual for each opponent - more than 5 total gambling attempts

        if (deaths == 1)
        {
            dialogueManager.EnterDialogueMode(firstDeathInkFile); // dialogue after our first ever death
        }
        else
        {
            int defeatedByCount = opponent?.numberOfDefeatsByOpponent ?? -1;

            if (defeatedByCount-1 <= numberOfDefeatTexts)
                dialogueManager.EnterDialogueMode(defeatInkFile, $"{defeatedBy}.died{defeatedByCount - 1}");
            else if (defeatedByCount-1 > numberOfDefeatTexts)
            {
                if (goodGamble > badGamble)
                {
                    if (totalGamble <= 5 && ggTriesLT5 < numberOfGambleTexts)
                    {
                        dialogueManager.EnterDialogueMode(goodGambleInkFile, $"lessThanFive.died{ggTriesLT5 + 1}");
                        opponent.goodGambleDialoguePlayedLessThanFive++;
                    }
                    if (totalGamble > 5 && ggTriesMT5 < numberOfGambleTexts)
                    {
                        dialogueManager.EnterDialogueMode(goodGambleInkFile, $"moreThanFive.died{ggTriesMT5 + 1}");
                        opponent.goodGambleDialoguePlayedMoreThanFive++;
                    }
                }
                else if (badGamble > goodGamble)
                {
                    if (totalGamble <= 5 && bgTriesLT5 < numberOfGambleTexts)
                    {
                        dialogueManager.EnterDialogueMode(badGambleInkFile, $"lessThanFive.died{bgTriesLT5 + 1}");
                        opponent.badGambleDialoguePlayedLessThanFive++;
                    }
                    if (totalGamble > 5 && bgTriesMT5 < numberOfGambleTexts)
                    {
                        dialogueManager.EnterDialogueMode(badGambleInkFile, $"moreThanFive.died{bgTriesMT5 + 1}");
                        opponent.goodGambleDialoguePlayedMoreThanFive++;
                    }
                }
                else
                {
                    Debug.LogWarning($"[NewRunDialogue] There is no more dialogue to choose from!");
                }
            }
        }
        data.SaveToJson();
    }
}
