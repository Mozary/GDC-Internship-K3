using UnityEngine;
using UnityEngine.SceneManagement;

public class ChapterLoader : MonoBehaviour
{
    [SerializeField] private string SelectScene;
    [SerializeField] private int SceneID;
    private SaveState State;
    private void Awake()
    {
        SaveSystem.init();
        State = SaveSystem.LoadGame();
    }
    public void LoadScene()
    {
        if (SelectScene != null && State.ChapterStates[SceneID].Completed == true)
        {
            Debug.Log("ACTIVATED ID: " + SceneID);
            State.ActiveChapter = SceneID;
            SaveSystem.SaveGame(State);
            SceneManager.LoadScene(SelectScene);
        }
    }
}
