using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionManager : MonoBehaviour
{
    [SerializeField] private float TypingSpeed = 0.05f;

    private TextMeshProUGUI DescriptionText;
    private Text TitleText;
    private Coroutine DisplayCoroutine;
    private int CurrentId = 0;
    private string CurrentTitle = "";
    private SaveState State;
    
    void Start()
    {
        State = SaveSystem.LoadGame();

        DescriptionText = transform.Find("Description").GetComponent<TextMeshProUGUI>();
        TitleText = transform.Find("Title").GetComponent<Text>();
        TitleText = transform.Find("Title").GetComponent<Text>();

        DescriptionText.text = "";
        TitleText.text = "";
    }
    IEnumerator TypeDescription(string Description)
    {
        foreach (char letter in Description.ToCharArray())
        {
            DescriptionText.text += letter;
            yield return new WaitForSeconds(TypingSpeed);
        }
    }
    IEnumerator TypeTitle(string Title)
    {
        foreach (char letter in Title.ToCharArray())
        {
            TitleText.text += letter;
            yield return new WaitForSeconds(TypingSpeed);
        }
    }
    IEnumerator ChangeImage(int id)
    {
        Transform Prev = transform.Find("Mask").Find(CurrentId.ToString());
        Transform Current = transform.Find("Mask").Find(id.ToString());

        Current.SetAsLastSibling();
        CurrentId = id;

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
    public void DisplayText(string text, string title, int id)
    {
        if (title != CurrentTitle)
        {
            if (id>= 0 && State.ChapterStates[id].Unlocked == false)
            {
                text = "You have not unlocked this chapter yet.";
                title = "???";
            }

            CurrentTitle = title;
            StopAllCoroutines();

            TitleText.text = "";
            DescriptionText.text = "";

            StartCoroutine(ChangeImage(id));
            StartCoroutine(TypeDescription(text));
            StartCoroutine(TypeTitle(title));
        }
    }
}
