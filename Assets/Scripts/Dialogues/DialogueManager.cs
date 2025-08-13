using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    public TextAsset dialogueForTesting;

    [Header("Dialogue Panel Components")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private Image speakerImage;

    //consider enums
    private const string SPEAKER_TAG = "speaker", PORTRAIT_TAG = "portrait";

    private Story currentStory;
	//make it so ite cant be changed from outside this class
	//example:
	//public bool IsDialoguePlaying { get; private set; } = false;
	public bool isDialoguePlaying; // true when dialogue is active
    public bool isDialoguePaused;  // true when some event started and dialogue has to pause

    [SerializeField] private float textDelay = .03f;

    // Needed for text generation
    bool isTextGenerating;
    string lineToGenerate;

    private void Awake()
    {
        if (instance != null)
            Debug.LogWarning("Found more than one Dialogue Manager in the scene!");

        instance = this;
    }

    public static DialogueManager GetInstance() => instance;
    //public static DialogueManager Instance => instance;

    private void Start()
    {
        isDialoguePaused = false;

        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);

        lineToGenerate = "";
    }

    //use new unity input system in the future
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            EnterDialogueMode(dialogueForTesting);
        }

        if (!isDialoguePlaying) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            InteractionWithDialogueBox();
        }
    }

    void InteractionWithDialogueBox()
    {
        if (isTextGenerating)
            SkipDialogueGeneration();
        else if (!isTextGenerating)
            ContinueStory();
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        Debug.Log("Entering Dialogue Mode");
        currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    public void EnterDialogueMode(TextAsset inkJSON, string knotName)
    {
        Debug.Log("Entering Dialogue Mode");
        currentStory = new Story(inkJSON.text);
        isDialoguePlaying = true;
        currentStory.ChoosePathString(knotName);
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    public void ExitDialogueMode()
    {
        dialoguePanel.SetActive(false);
        isDialoguePlaying = false;
        isDialoguePaused = false;
        dialogueText.text = "";
        currentStory = null;
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (!isDialoguePaused)
            { 
                lineToGenerate = currentStory.Continue();
                StartCoroutine("DialogueDisplay");

                HandleTags(currentStory.currentTags);
            }
        }
        else
            ExitDialogueMode();
    }

    private void HandleTags(List<string> currentTags)
    {
        // loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
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

    IEnumerator DialogueDisplay()
    {
        isTextGenerating = true;

        dialogueText.text = lineToGenerate;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i < dialogueText.text.Length; i++)
        {
            dialogueText.maxVisibleCharacters++;
            yield return new WaitForSeconds(textDelay);
        }

        isTextGenerating = false;
    }

    private void SkipDialogueGeneration()
    {
        StopCoroutine("DialogueDisplay");
        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        isTextGenerating = false;
    }

    public void FastSkip()     // only for debbuging and testing, skips whole dialogue
    {
        float ogTextDelay = textDelay;
        textDelay = 0;
        while (currentStory.canContinue)
        {
            ContinueStory();
        }
        if (!currentStory.canContinue)
            textDelay = ogTextDelay;
    }
}
