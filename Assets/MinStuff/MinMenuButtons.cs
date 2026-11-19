using UnityEngine;
using UnityEngine.SceneManagement;

public class MinMenuButtons : MonoBehaviour
{
    public GameObject mainPage;
    public GameObject helpPage;
    public string gameScene = "Assets/MinStuff/MinTokenTest.unity";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowHelp()
    {
        mainPage.SetActive(false);
        helpPage.SetActive(true);
    }

    public void Back()
    {
        helpPage.SetActive(false);
        mainPage.SetActive(true);
    }

    public void Play()
    {
#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
            gameScene, new LoadSceneParameters(LoadSceneMode.Single));
#else
        SceneManager.LoadScene(gameScene);
#endif
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
