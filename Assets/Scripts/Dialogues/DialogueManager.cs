using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : PersistenSingleton<DialogueManager>
{
    private EnemyTease enemyTease;
    private NewRunDialogue newRunDialogue;
    [SerializeField] private DialogueData data;

    [Header("Dialogue Panel Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private Image speakerImage;
    [SerializeField] private Button skipDialogueButton;
    [SerializeField] private Button increaseGoodGamblingBTN, increaseBadGamblingBTN;

    private const string SPEAKER_TAG = "speaker", PORTRAIT_TAG = "portrait";

    private Story _currentStory;
    public bool isDialoguePlaying; 
    public bool isDialoguePaused;  // true when some event started and dialogue has to pause
    private bool _isTextGenerating;

    [SerializeField] private float _textDelay = .003f;

    [SerializeField] private TextAsset TESTINKJSON;

    private void Start()
    {
        data = GetComponent<DialogueData>();

        skipDialogueButton.onClick.AddListener(SkipAll);

        enemyTease = GetComponent<EnemyTease>();
        newRunDialogue = GetComponent<NewRunDialogue>();

        isDialoguePaused = false;

        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);

        increaseBadGamblingBTN.onClick.AddListener(data.IncreaseBadGambleAttempts);
        increaseGoodGamblingBTN.onClick.AddListener(data.IncreaseGoodGambleAttempts);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            EnterDefeatDialogueMode("Minotaur");

        if (Input.GetKeyDown(KeyCode.P))
            EnterDialogueMode(TESTINKJSON);
    }

    public void InteractionWithDialogueBox()
    {
        if (_isTextGenerating)
            SkipDialogueGeneration();
        else if (!_isTextGenerating)
            ContinueStory();
    }

    public void EnterDefeatDialogueMode(string defeatedBy)=>
        newRunDialogue.DefeatDialogue(defeatedBy);
    

    #region Enter dialogue methods
    // Start dialogue from file
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    // Start dialogue from file, at certain knot
    public void EnterDialogueMode(TextAsset inkJSON, string knotName)
    {
        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        _currentStory.ChoosePathString(knotName);
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    // Start dialogue from path
    public void EnterDialogueMode(string path)
    {
        TextAsset inkJSON = Resources.Load<TextAsset>(path);

        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    // Start dialogue from path, at certain knot
    public void EnterDialogueMode(string path, string knotName)
    {
        TextAsset inkJSON = Resources.Load<TextAsset>(path);

        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        _currentStory.ChoosePathString(knotName);
        dialoguePanel.SetActive(true);

        ContinueStory();
    }
    #endregion

    private void ExitDialogueMode()
    {
        dialoguePanel.SetActive(false);
        isDialoguePlaying = false;
        isDialoguePaused = false;
        dialogueText.text = "";
        _currentStory = null;
    }


    private void ContinueStory()
    {
        if (_currentStory.canContinue && !isDialoguePaused)
        {
            string line = _currentStory.Continue();

            HandleTags(_currentStory.currentTags);
            HandleFunctions(line);
            
            StartCoroutine("DialogueDisplay", line);            
        }
        else
            ExitDialogueMode();
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (string tag in currentTags)
        {
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriately parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case SPEAKER_TAG:
                    speakerName.text = tagValue;
                    break;
                case PORTRAIT_TAG:
                    if (tagValue == "null")
                    {
                        speakerImage.enabled = false;
                        break;
                    }
                    speakerImage.enabled = true;
                    string[] splitPortraitName = tagValue.Split("_");
                    Sprite sprite = Resources.Load<Sprite>($"Dialogue Sprites/{splitPortraitName[0].Trim()}/{splitPortraitName[1].Trim()}");
                    speakerImage.sprite = sprite;
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void HandleFunctions(string currentLine)
    {
        if (currentLine[0] != '>' && currentLine[2] != '>')
            return;

        string methodName = currentLine.TrimStart('>');

        Debug.LogWarning($"Trying to call method named {methodName}");

        switch (methodName)
        {
            default:
                Debug.LogWarning("There is no method called " + methodName);
                break;
        }
    }


    IEnumerator DialogueDisplay(string line)
    {
        _isTextGenerating = true;

        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i < dialogueText.text.Length; i++)
        {
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(_textDelay);
        }

        _isTextGenerating = false;
    }

    private void SkipDialogueGeneration()
    {
        StopCoroutine("DialogueDisplay");
        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        _isTextGenerating = false;
    }

    public void SkipAll()     // only for debbuging and testing, skips whole dialogue
    {
        float ogTextDelay = _textDelay;
        _textDelay = 0;
        while (_currentStory.canContinue)
        {
            ContinueStory();
        }
        if (!_currentStory.canContinue)
            _textDelay = ogTextDelay;
    }

    public void ShowEnemyTease() => enemyTease.Tease();
}
