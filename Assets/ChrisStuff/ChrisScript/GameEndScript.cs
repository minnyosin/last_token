using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameEndScript : MonoBehaviour
{
    public GameObject GameEndScreen;

    public Image redScreen;
    public Image gameOverText;
    public Animator buttonAnim;
    bool playerisHere = false;
    void Awake()
    {
        GameEndScreen.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerisHere) return;
            StartCoroutine(GameEnd());
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    IEnumerator GameEnd()
    {
        playerisHere = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        SetAlpha(redScreen, 0f);
        SetAlpha(gameOverText, 0f);
        GameEndScreen.SetActive(true);


        yield return StartCoroutine(FadeImage(redScreen, 0f, 1f, 1f));

 
        yield return new WaitForSecondsRealtime(1f);

        yield return StartCoroutine(FadeImage(gameOverText, 0f, 1f, 1f));


        yield return new WaitForSecondsRealtime(1f);
        buttonAnim.SetBool("isGameOver", true);
    }

    IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(img, Mathf.Lerp(from, to, t / duration));
            yield return null;
        }
        SetAlpha(img, to);
    }

    void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    public void QuitGame()
    {
        
        SceneManager.LoadScene(0);
    }
}
