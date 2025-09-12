using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : PersistenSingleton<DialogueManager>
{
    private EnemyTease _enemyTease;
    private NewRunDialogue _newRunDialogue;
    [SerializeField] private DialogueData _data;

    [Header("Dialogue Panel Components")][Space]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _speakerName;
    [SerializeField] private Image _speakerImage;
    [SerializeField] private Button _skipDialogueButton;
    [SerializeField] private Button _increaseGoodGamblingBTN, _increaseBadGamblingBTN;
    [SerializeField] private Sprite _athenaSprite, _apistosSprite;
    private const string SPEAKER_TAG = "speaker", PORTRAIT_TAG = "portrait";

    [Header("Flags")][Space]
    public bool IsDialoguePlaying; 
    public bool IsDialoguePaused;
    private bool _isTextGenerating;
    private Story _currentStory;


    [Header("Text Generating Properties")][Space]
    [SerializeField] private float _textDelay = .03f;

    [SerializeField] private TextAsset testText;

    private void Start()
    {
        _data = GetComponent<DialogueData>();

        _skipDialogueButton.onClick.AddListener(SkipAll);

        _enemyTease = GetComponent<EnemyTease>();
        _newRunDialogue = GetComponent<NewRunDialogue>();

        IsDialoguePaused = false;

        IsDialoguePlaying = false;
        _dialoguePanel.SetActive(false);

        _increaseBadGamblingBTN?.onClick.AddListener(_data.IncreaseBadGambleAttempts);
        _increaseGoodGamblingBTN?.onClick.AddListener(_data.IncreaseGoodGambleAttempts);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            EnterDialogueMode(testText);
    }

    public void InteractionWithDialogueBox()
    {
        if (_isTextGenerating)
            SkipDialogueGeneration();
        else if (!_isTextGenerating)
            ContinueStory();
    }

    public void EnterDefeatDialogueMode(string defeatedBy)=>
        _newRunDialogue.DefeatDialogue(defeatedBy);
    

    #region Enter dialogue methods
    // Start dialogue from file
    public void EnterDialogueMode(TextAsset inkJSON, string knotName = null)
    {
        _currentStory = new Story(inkJSON.text);
        IsDialoguePlaying = true;

        if(knotName != null)
            _currentStory.ChoosePathString(knotName);

        _dialoguePanel.SetActive(true);

        ContinueStory();
    }

    // Start dialogue from path
    public void EnterDialogueMode(string path, string knotName = null)
    {
        TextAsset inkJSON = Resources.Load<TextAsset>(path);

        if (knotName != null)
            _currentStory.ChoosePathString(knotName);

        _currentStory = new Story(inkJSON.text);
        _dialoguePanel.SetActive(true);
        IsDialoguePlaying = true;

        ContinueStory();
    }
    #endregion

    private void ExitDialogueMode()
    {
        _dialoguePanel.SetActive(false);
        IsDialoguePlaying = false;
        IsDialoguePaused = false;
        _dialogueText.text = "";
        _currentStory = null;
    }


    private void ContinueStory()
    {
        if (_currentStory.canContinue && !IsDialoguePaused)
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
                    _speakerName.text = tagValue;
                    _speakerImage.sprite = tagValue == "Athena" ? _athenaSprite : _apistosSprite;
                    break;
                case PORTRAIT_TAG:
                    if (tagValue == "null")
                    {
                        _speakerImage.enabled = false;
                        break;
                    }
                    _speakerImage.enabled = true;
                    string[] splitPortraitName = tagValue.Split("_");
                    Sprite sprite = Resources.Load<Sprite>($"Dialogue Sprites/{splitPortraitName[0].Trim()}/{splitPortraitName[1].Trim()}");
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

        _dialogueText.text = line;
        _dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i < _dialogueText.text.Length; i++)
        {
            _dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(_textDelay);
        }

        _isTextGenerating = false;
    }

    private void SkipDialogueGeneration()
    {
        StopCoroutine("DialogueDisplay");
        _dialogueText.maxVisibleCharacters = _dialogueText.text.Length;
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

    public void ShowEnemyTease() => _enemyTease.Tease();
}
