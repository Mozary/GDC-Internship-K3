using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChapterLoader : MonoBehaviour
{
    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioClip MenuSelect;
    [SerializeField] private AudioClip MenuHover;

    [SerializeField] private bool IsAChapterMenu = false;
    [SerializeField] private bool IsAGameplayMenu = false;
    private SaveState State;
    private GameObject FadingLayer;
    private bool InTransition = false;
    private void Awake()
    {
        SaveSystem.init();
        FadingLayer = GameObject.Find("FadingLayer");
        State = SaveSystem.LoadGame();
        if (IsAChapterMenu)
        {
            foreach (Transform child in transform)if(child.CompareTag("Chapter"))
            {
                int id = child.GetComponent<OnHoverDescription>().GetId();
                Button childbutton = child.Find("Button").GetComponent<Button>();
                if (!State.ChapterStates[id].Unlocked)
                {
                    childbutton.interactable = false;
                }
            }
        }
    }
    private void Start()
    {
        if (!IsAGameplayMenu)
        {
            StartCoroutine(FadeIn());
        }
    }
    IEnumerator FadeIn()
    {
        FadingLayer.transform.SetAsLastSibling();
        Color mycolour = Color.black;
        float colourfade = 1;
        while (colourfade > 0)
        {
            colourfade -= 0.03f;
            mycolour.a = colourfade;
            FadingLayer.GetComponent<Image>().color = mycolour;
            yield return null;
        }
        FadingLayer.transform.SetAsFirstSibling();
    }
    IEnumerator FadeOut(string Destination)
    {
        FadingLayer.transform.SetAsLastSibling();
        Color mycolour = Color.black;
        float colourfade = 0;
        while (colourfade < 1)
        {
            colourfade += 0.03f;
            mycolour.a = colourfade;
            FadingLayer.GetComponent<Image>().color = mycolour;
            yield return null;
        }
        Debug.Log("Continue To " + Destination);
        SceneManager.LoadScene(Destination);
    }

    public void IsDefeated(bool TimesUp)
    {
        if (!IsAChapterMenu)
        {
            transform.Find("Status").gameObject.SetActive(true);
            transform.Find("Button_Retreat").gameObject.SetActive(true);
            transform.Find("Button_Restart").gameObject.SetActive(true);
            transform.Find("Button_Continue").gameObject.SetActive(false);

            Text Status = transform.Find("Status").GetComponent<Text>();
            Status.text = "YOU ARE DEFEATED";
            if (TimesUp)
            {
                Status.text = "YOUR SISTER PERISHED";
            }
            StartCoroutine(FadeIn());
        }
    }
    public void IsVictorious()
    {
        if (!IsAChapterMenu)
        {
            transform.Find("Status").gameObject.SetActive(true);
            transform.Find("Button_Retreat").gameObject.SetActive(true);
            transform.Find("Button_Continue").gameObject.SetActive(true);
            transform.Find("Button_Restart").gameObject.SetActive(false);

            Text Status = transform.Find("Status").GetComponent<Text>();
            Status.text = "YOU ARE VICTORIOUS!";
            StartCoroutine(FadeIn());
        }
    }
    public void HoverSound()
    {
        Audio.PlayOneShot(MenuSelect);
    }
    public void RestartScene()
    {
        if (!InTransition && !IsAChapterMenu)
        {
            Audio.PlayOneShot(MenuSelect);
            Debug.Log("RESTARTING");
            State = SaveSystem.LoadGame();
            InTransition = true;
            string destination = SceneManager.GetActiveScene().name;
            StartCoroutine(FadeOut(destination));
        }
    }
    public void LoadScene(string SelectScene)
    {
        int SceneID = 0;
        switch (SelectScene)
        {
            case "Prelude":
                SceneID = 0;
                break;
            case "Chapter 1":
                SceneID = 1;
                break;
            case "Chapter 2":
                SceneID = 2;
                break;
            case "Chapter 3":
                SceneID = 3;
                break;
            case "EndGame":
                SceneID = 4;
                break;
            default:
                SceneID = 0;
                break;
        }
        State = SaveSystem.LoadGame();
        if (SelectScene != null && State.ChapterStates[SceneID].Unlocked == true && !InTransition)
        {
            Audio.PlayOneShot(MenuSelect);
            InTransition = true;
            State.ActiveChapter = SceneID;
            SaveSystem.SaveGame(State);
            StartCoroutine(FadeOut(SelectScene));
        }
        else
        {
            Debug.Log("SCENE CAN'T BE LOADED");
        }
    }
}
