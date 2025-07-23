using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DialogueSO", fileName = "DialogueData")]
public class DialogueSO : ScriptableObject
{
    public List<DialogueLevelDetail> DialogueLevelDetails;
}

[Serializable] 
public class DialogueLevelDetail
{
    public int levelId;
    public List<DialogueLine> dialogueLineBefore;
    public List<DialogueLine> dialogueLineAfter;
 }

[Serializable]
public class DialogueLine
{
    public string speakerName;
    public Sprite speakerIcon;
    [TextArea(2, 5)]
    public string sentence;
}
