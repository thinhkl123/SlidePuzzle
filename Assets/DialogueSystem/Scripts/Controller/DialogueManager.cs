using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUtils;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : SingletonMono<DialogueManager>
{
    [Header(" Config ")]
    [SerializeField] private DialogueSO DialogueData;
    [SerializeField] private int levelId;
    [SerializeField] private bool isAfter;
    [SerializeField] private RescueCreature rescueCreature;

    [Header(" Dialogue Panel ")]
    public TextMeshProUGUI speakerNameText;
    public Image speakerIcon;
    public TextMeshProUGUI sentenceText;
    public GameObject dialogueBox;

    private Queue<DialogueLine> dialogueLines = new Queue<DialogueLine>();
    private bool isTyping = false;
    private string currentSentence;
    private bool isStarted = false;

    public float typeSpeed = 0.03f;

    private void Start()
    {
        if (isAfter)
        {
            rescueCreature.PlayRescueEffect();
            Invoke(nameof(StartDialogueThisState), 2f);
        }
    }

    public void StartDialogueThisState()
    {
        DialogueLevelDetail levelDetail = DialogueData.DialogueLevelDetails[levelId - 1];
        if (isAfter)
        {
            StartDialogue(levelDetail.dialogueLineAfter);
        }
        else
        {
            StartDialogue(levelDetail.dialogueLineBefore);
        }
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        isStarted = true;

        dialogueBox.SetActive(true);
        dialogueLines.Clear();

        foreach (DialogueLine line in lines)
        {
            dialogueLines.Enqueue(line);
        }

        DisplayNextLine();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isStarted)
        {
            OnNextButton();
        }
    }

    public void DisplayNextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            sentenceText.text = currentSentence;
            isTyping = false;
            return;
        }

        if (dialogueLines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines.Dequeue();
        speakerNameText.text = line.speakerName;
        speakerIcon.sprite = line.speakerIcon;
        StartCoroutine(TypeSentence(line.sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        currentSentence = sentence;
        sentenceText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            sentenceText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        if (isAfter)
        {
            //Load next level
            this.LoadLevel();
        }
        else
        {
            //Load to puzzle
            LoadingManager.instance.LoadScene("Puzzle");
        }
    }

    public void LoadLevel()
    {
        int currentLevel = PlayerPrefs.GetInt(Constant.LEVELID, 1);
        //LoadingManager.instance.LoadScene("Level " + currentLevel);
        LoadingManager.instance.LoadScene("Level 1");
    }

    public void NextLevel()
    {
        int currentLevel = PlayerPrefs.GetInt(Constant.LEVELID, 1);
        int nextLevel = currentLevel + 1;
        PlayerPrefs.SetInt(Constant.LEVELID, nextLevel);
        PlayerPrefs.Save();
        //LoadingManager.instance.LoadScene("currentLevel " + nextLevel + " After");
        LoadingManager.instance.LoadScene("Level 1 After");
    }

    // Gọi hàm này khi người chơi bấm nút Next
    public void OnNextButton()
    {
        DisplayNextLine();
    }
}
