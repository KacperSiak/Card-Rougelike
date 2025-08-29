using System.Collections.Generic;
using UnityEngine;

public class DialogueData : MonoBehaviour
{
    public OverallDialogueInformation OverallDialogueInformation;

    public void SaveToJson()
    {
        string dialogueInformation = JsonUtility.ToJson(OverallDialogueInformation);
        string filePath = Application.persistentDataPath + "/DialogueData.json";
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, dialogueInformation);
    }

    public void LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/DialogueData.json";
        string dialogueInformation = System.IO.File.ReadAllText(filePath);

        OverallDialogueInformation = JsonUtility.FromJson<OverallDialogueInformation>(dialogueInformation);
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
    public string opponentName;
    public int numberOfDefeatsByOpponent;
    public int goodGambleDialoguePlayed;
    public int badGambleDialoguePlayed;
}
