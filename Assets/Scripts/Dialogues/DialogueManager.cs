using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    private EnemyTease _enemyTease;
    private NewRunDialogue _newRunDialogue;

    [Header("Dialogue Panel Components")]
    [SerializeField] private GameObject _dialoguePanel;
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private TextMeshProUGUI _speakerName;
    [SerializeField] private Image _speakerImage;

    private const string SPEAKER_TAG = "speaker", PORTRAIT_TAG = "portrait";

    private Story _currentStory;
    public bool isDialoguePlaying; 
    public bool isDialoguePaused;  // true when some event started and dialogue has to pause
    private bool _isTextGenerating;

    [SerializeField] private float _textDelay = .003f;

    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Found more than one Dialogue Manager in the scene!");
        else
            instance = this;
    }

    public static DialogueManager GetInstance() => instance;

    private void Start()
    {
        _enemyTease = GetComponent<EnemyTease>();
        _newRunDialogue = GetComponent<NewRunDialogue>();

        isDialoguePaused = false;

        isDialoguePlaying = false;
        _dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isDialoguePlaying) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            InteractionWithDialogueBox();
        }
    }

    public void InteractionWithDialogueBox()
    {
        if (_isTextGenerating)
            SkipDialogueGeneration();
        else if (!_isTextGenerating)
            ContinueStory();
    }

    public void DialogueByDefeat(string defeatedBy)=>
        _newRunDialogue.DialogueByDefeat(defeatedBy);
    

    #region Enter dialogue methods
    // Start dialogue from file
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        _dialoguePanel.SetActive(true);

        ContinueStory();
    }

    // Start dialogue from path
    public void EnterDialogueMode(string path)
    {
        TextAsset inkJSON = Resources.Load<TextAsset>(path);

        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        _dialoguePanel.SetActive(true);

        ContinueStory();
    }

    // Start dialogue from file, at certain knot
    public void EnterDialogueMode(TextAsset inkJSON, string knotName)
    {
        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        _currentStory.ChoosePathString(knotName);
        _dialoguePanel.SetActive(true);

        ContinueStory();
    }

    // Start dialogue from path, at certain knot
    public void EnterDialogueMode(string path, string knotName)
    {
        TextAsset inkJSON = Resources.Load<TextAsset>(path);

        _currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        _currentStory.ChoosePathString(knotName);
        _dialoguePanel.SetActive(true);

        ContinueStory();
    }
    #endregion

    private void ExitDialogueMode()
    {
        _dialoguePanel.SetActive(false);
        isDialoguePlaying = false;
        isDialoguePaused = false;
        _dialogueText.text = "";
        _currentStory = null;
    }

    private void ContinueStory()
    {
        if (_currentStory.canContinue)
        {
            if (!isDialoguePaused)
            { 
                //_lineToGenerate = _currentStory.Continue();
                StartCoroutine("DialogueDisplay", _currentStory.Continue());

                HandleTags(_currentStory.currentTags);
            }
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

            // handle the tag
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    _speakerName.text = tagValue;
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
                    _speakerImage.sprite = sprite;
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
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

    public void FastSkip()     // only for debbuging and testing, skips whole dialogue
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
