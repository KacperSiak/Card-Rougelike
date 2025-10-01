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
    [SerializeField] private TextAsset equalGambleInkFile;
    [SerializeField] private TextAsset neutralInkFile;

    [SerializeField] int numberOfDefeatTexts, numberOfGambleTexts;


    [SerializeField] string defeatedBy; // for testing


    private void Start()
    {
        dialogueManager = DialogueManager.Instance;
        data = GetComponent<DialogueData>();
        info = data.OverallDialogueInformation;
    }

    public void DefeatDialogue(string defeatedBy)
    {
        if (dialogueManager.IsDialoguePlaying) return;

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
        int eqTries = opponent?.equalGambleDialoguePlayed ?? -1;                // equal gamble tries
        int neutralTries = opponent?.neutralDialoguePlayed ?? -1;               // dialogues after every other has played

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
                        Dialogue(goodGambleInkFile, $"lessThanFive.died{ggTriesLT5 + 1}", opponent.goodGambleDialoguePlayedLessThanFive);
                    }
                    if (totalGamble > 5 && ggTriesMT5 < numberOfGambleTexts)
                    {
                        Dialogue(goodGambleInkFile, $"moreThanFive.died{ggTriesMT5 + 1}", opponent.goodGambleDialoguePlayedMoreThanFive);
                    }
                }
                else if (badGamble > goodGamble)
                {
                    if (totalGamble <= 5 && bgTriesLT5 < numberOfGambleTexts)
                    {
                        Dialogue(badGambleInkFile, $"lessThanFive.died{bgTriesLT5 + 1}", opponent.badGambleDialoguePlayedLessThanFive);
                    }
                    if (totalGamble > 5 && bgTriesMT5 < numberOfGambleTexts)
                    {
                        Dialogue(badGambleInkFile, $"moreThanFive.died{bgTriesMT5 + 1}", opponent.goodGambleDialoguePlayedMoreThanFive);
                    }
                }
                else if (badGamble == goodGamble && eqTries < numberOfGambleTexts)
                {
                    Dialogue(equalGambleInkFile, $"main.died{eqTries + 1}", opponent.equalGambleDialoguePlayed++);
                }
                else
                {
                    if (neutralTries < 3)
                    {
                        Dialogue(neutralInkFile, $"main.died{neutralTries + 1}", opponent.neutralDialoguePlayed);
                    }
                    else
                        dialogueManager.EnterDialogueMode(neutralInkFile, $"main.died3");
                }
            }
        }
        data.SaveToJson();
    }

    void Dialogue(TextAsset asset, string knot, int value)
    {
        dialogueManager.EnterDialogueMode(asset, knot);
        value++;
    }
}