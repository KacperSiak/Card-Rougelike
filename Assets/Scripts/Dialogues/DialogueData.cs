using Ink.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueData : MonoBehaviour
{
    public OverallDialogueInformation OverallDialogueInformation;

    [SerializeField] private TextAsset dialogueVariablesInkFile;
    private Story story;

    private void Start()=>
        story = new Story(dialogueVariablesInkFile.text);

    public void SaveToJson()
    {
        string dialogueInformation = JsonUtility.ToJson(OverallDialogueInformation);
        string filePath = Application.persistentDataPath + "DialogueData.json";
        System.IO.File.WriteAllText(filePath, dialogueInformation);
    } 

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "DialogueData.json";
        string dialogueInformation = System.IO.File.ReadAllText(filePath);

        OverallDialogueInformation = JsonUtility.FromJson<OverallDialogueInformation>(dialogueInformation);
    }

    public void IncreaseGoodGambleAttempts()
    {
        OverallDialogueInformation.goodGambleAttempts++;
        OverallDialogueInformation.allGambleAttempts++;

        story.variablesState["goodGamblingAttempts"] = OverallDialogueInformation.goodGambleAttempts;
        story.variablesState["totalGamblingAttempts"] = OverallDialogueInformation.allGambleAttempts;

        SaveToJson();
    }

    public void IncreaseBadGambleAttempts()
    {
        OverallDialogueInformation.badGambleAttempts++;
        OverallDialogueInformation.allGambleAttempts++;

        story.variablesState["totalGamblingAttempts"] = OverallDialogueInformation.allGambleAttempts;
        story.variablesState["badGamblingAttempts"]   = OverallDialogueInformation.badGambleAttempts;

        SaveToJson();
    }

    public void ClearGambleAttempts()
    {
        OverallDialogueInformation.allGambleAttempts  = 0;
        OverallDialogueInformation.badGambleAttempts  = 0;
        OverallDialogueInformation.goodGambleAttempts = 0;
    }

    public void IncreaseDeathCount(string defeatedBy)
    {
        var oppoonent = OverallDialogueInformation.opponentInformation.FirstOrDefault(c => c.opponentName == defeatedBy);
        oppoonent.numberOfDefeatsByOpponent++;
        OverallDialogueInformation.numberOfDeaths++;

        SaveToJson();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SaveToJson();

        if(Input.GetKeyDown(KeyCode.L))
            LoadFromJson();
    }
}


[System.Serializable]
public class OverallDialogueInformation
{
    public int numberOfDeaths;

    public int goodGambleAttempts;
    public int badGambleAttempts;
    public int allGambleAttempts;

    public List<OpponentInformation> opponentInformation = new List<OpponentInformation>();
}

[System.Serializable]
public class OpponentInformation
{
    // potne sie
    public string opponentName;
    public int numberOfDefeatsByOpponent;
    public int goodGambleDialoguePlayedMoreThanFive;
    public int goodGambleDialoguePlayedLessThanFive;
    public int badGambleDialoguePlayedMoreThanFive;
    public int badGambleDialoguePlayedLessThanFive;
    public int equalGambleDialoguePlayed;
    public int neutralDialoguePlayed;
}
