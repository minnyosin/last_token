using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public GameObject UIImage;
    public AudioSource buttonSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        buttonSound.Play();
        UIImage.SetActive(true);
        Debug.Log("Resume");
    }
    public void Exit()
    {
        Debug.Log("Quit");
    }
}
