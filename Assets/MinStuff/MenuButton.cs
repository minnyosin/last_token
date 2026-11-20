using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public GameObject mainPage;
    public GameObject helpPage;
    public string gameScene = "Assets/MinStuff/MinTokenTest.unity";

    public AudioSource music;
    public AudioSource sounds;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public AudioClip startSound;
    public Animator transition;
    public CanvasGroup transitionCover;
    public float transitionTime = .5f;

    bool loading;
    float lastHover = -1;
    float lastClick = -1;

    void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(transitionCover != null)
        {
            transitionCover.blocksRaycasts = false;
        }
        if(music != null && music.clip != null)
        {
            music.Play();
        }
    }

    public void Hover()
    {
        if(loading || Time.unscaledTime - lastHover < .12f || Time.unscaledTime - lastClick < .2f)
        {
            return;
        }
        lastHover = Time.unscaledTime;
        if(sounds != null && hoverSound != null)
        {
            sounds.Stop();
            sounds.clip = hoverSound;
            sounds.Play();
        }
    }

    public void Click()
    {
        if(loading)
        {
            return;
        }
        if(sounds != null && clickSound != null)
        {
            lastClick = Time.unscaledTime;
            sounds.Stop();
            sounds.clip = clickSound;
            sounds.Play();
        }
    }

    public void ShowHelp()
    {
        if(loading)
        {
            return;
        }
        mainPage.SetActive(false);
        helpPage.SetActive(true);
    }

    public void Back()
    {
        if(loading)
        {
            return;
        }
        helpPage.SetActive(false);
        mainPage.SetActive(true);
    }

    public void Play()
    {
        if(loading)
        {
            return;
        }
#if UNITY_EDITOR
        if(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(gameScene) == null)
#else
        if(!Application.CanStreamedLevelBeLoaded(gameScene))
#endif
        {
            Debug.LogError("Check Game Scene on Menu Buttons. Also add the scene to the build scene list.");
            return;
        }
        loading = true;
        StartCoroutine(OpenGame());
    }

    IEnumerator OpenGame()
    {
        if(sounds != null && startSound != null)
        {
            sounds.Stop();
            sounds.clip = startSound;
            sounds.Play();
        }
        if(transitionCover != null)
        {
            transitionCover.blocksRaycasts = true;
        }
        if(transition != null)
        {
            transition.SetTrigger("Close");
        }

        float timer = 0;
        float startingVolume = 0;
        if(music != null)
        {
            startingVolume = music.volume;
        }
        while(timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            if(music != null)
            {
                music.volume = Mathf.Lerp(startingVolume,0,timer/transitionTime);
            }
            yield return null;
        }

#if UNITY_EDITOR
        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
            gameScene,new LoadSceneParameters(LoadSceneMode.Single));
#else
        AsyncOperation load = SceneManager.LoadSceneAsync(gameScene);
        while(!load.isDone)
        {
            yield return null;
        }
#endif
    }

    public void Quit()
    {
        if(loading)
        {
            return;
        }
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
