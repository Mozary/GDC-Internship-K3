using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string SceneName;
    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
}
