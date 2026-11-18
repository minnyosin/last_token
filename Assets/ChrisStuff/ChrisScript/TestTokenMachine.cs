using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class TestTokenMachine : MonoBehaviour, IInteractable
{
    public TextMeshProUGUI UIText;
    public Animator textAnim;
    public Animator exitDoor;
    public GameObject p;
    public PlayerInteractor player;
    public AudioSource insertAudio;
    int tokenContain = 0;
    bool stopLoop=false;
   // public GameObject bulletEffect;
    void Awake()
    {
        p = GameObject.FindGameObjectWithTag("Player");
        player=p.GetComponent<PlayerInteractor>();
    }
    public void Interact()
    {
        Debug.Log("hello");
        if (player.carriedTokens <= 0) return;
        
        StartCoroutine(FillingToken());
    }

    // Update is called once per frame
    void Update()
    {
        if (tokenContain == 4)
        {
            if (stopLoop) { return; }

            exitDoor.SetBool("isExitOpen", true);
            stopLoop = true;
        }
    }
    IEnumerator FillingToken()
    {
        insertAudio.Play();
        player.carriedTokens--;
        tokenContain++;
        UIText.SetText($"{tokenContain}/4");
        textAnim.SetTrigger("isInsert");
        yield return null;

    }
}
