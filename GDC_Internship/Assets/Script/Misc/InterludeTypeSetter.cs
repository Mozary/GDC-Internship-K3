using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterludeTypeSetter : MonoBehaviour
{
    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioClip TextClick;
    [SerializeField] private TextAsset DialogueAsset;
    [SerializeField] private ChapterLoader ChapLoader;
    [SerializeField] private string NextChapter;
    [SerializeField] private string DesignatedDialogue;
    [SerializeField] private float TypingSpeed = 0.05f;

    private DialogueScript LoadedDialogue;
    private DialogueLine[] LoadedDialogueLine;

    private TextMeshProUGUI TextDialogue;
    private Text TextName;
    private SaveState State;
    
    private string CurrentLine;
    private string CurrentName;
    private int CurrentLineNum = 0;
    private bool IsTyping = false;
    
    void Start()
    {
        State = SaveSystem.LoadGame();

        TextDialogue = transform.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        TextName = transform.Find("Name").GetComponent<Text>();

        TextDialogue.text = "";
        TextName.text = "";
        LoadedDialogue = JsonUtility.FromJson<DialogueScript>(DialogueAsset.text);

        switch (DesignatedDialogue)
        {
            case "Prelogue":
                LoadedDialogueLine = LoadedDialogue.Prelogue;
                break;
            case "Chapter 1":
                LoadedDialogueLine = LoadedDialogue.Chapter1;
                break;
            case "Chapter 2":
                LoadedDialogueLine = LoadedDialogue.Chapter2;
                break;
            case "Chapter 3":
                LoadedDialogueLine = LoadedDialogue.Chapter3;
                break;
            case "EndGame":
                LoadedDialogueLine = LoadedDialogue.Chapter3;
                break;
            default:
                LoadedDialogueLine = LoadedDialogue.Prelogue;
                break;
        }
        LoadNextLine();
    }
    IEnumerator TypeDialogue(string Description)
    {
        foreach (char letter in Description.ToCharArray())
        {
            TextDialogue.text += letter;
            Audio.PlayOneShot(TextClick);
            yield return new WaitForSeconds(TypingSpeed);
        }
    }
    IEnumerator ChangeImage(int id)
    {
        Transform Prev = transform.Find("Mask").Find(CurrentLineNum.ToString());
        Transform Current = transform.Find("Mask").Find(id.ToString());

        Current.SetAsLastSibling();
        CurrentLineNum = id;

        Color CurColour = Color.white;
        Color PrevColour = Color.white;
        float colourfadeOut = 1;
        float colourfadeIn = 0;

        while (colourfadeIn < 1)
        {
            colourfadeIn += 0.03f;
            colourfadeOut -= 0.03f;

            PrevColour.a = colourfadeOut;
            CurColour.a = colourfadeIn;

            Prev.GetComponent<Image>().color = PrevColour;
            Current.GetComponent<Image>().color = CurColour;
            yield return null;
        }

        yield return null;
    }
    public void LoadNextLine()
    {
        if(CurrentLineNum<= LoadedDialogueLine.Length)
        {
            CurrentLine = LoadedDialogueLine[CurrentLineNum].line;
            CurrentName = LoadedDialogueLine[CurrentLineNum].name;
            if (IsTyping)
            {
                StopAllCoroutines();
            }
            else if (!IsTyping)
            {
                DisplayText(CurrentLine, CurrentName, CurrentLineNum);
            }
        }
        else
        {
            ChapLoader.LoadScene(NextChapter);
        }
    }
    public void DisplayText(string Dialogue, string Name, int id)
    {
        StopAllCoroutines();
        TextName.text = "";
        TextDialogue.text = "";

        //StartCoroutine(ChangeImage(id));
        StartCoroutine(TypeDialogue(Dialogue));
        TextName.text = Name;
    }
}
