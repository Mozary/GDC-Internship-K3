using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingsLoader : MonoBehaviour
{
    [SerializeField] private string SelectScene;

    public void LoadScene()
    {
        if (SelectScene != null)
        {
            SceneManager.LoadScene(SelectScene);
        }
        
    }
}
