using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class PlayerInteractor: MonoBehaviour
{
    public LevelManagerScript LMS;
    public GameObject cineMachine1st;
    public GameObject cineMachine3rd;

    public int carriedTokens;
    public bool holdingScrewdriver=false;
    public bool playerDetected = false;
    public bool playeratVent = false;
    public bool playerInsideVent = false;
    public GameObject screwDriverUI;
    public GameObject ventTextUI;
    private IInteractable currentInteractable;

    public PlayerMovement playerMovement;
    public VentInteractionScript Vent;
    public PlayerCameraChangeScript PlayerCameraChange;
    int ventIndex=-1;
    public AudioSource ventIn;
    public AudioSource ventOut;
    public AudioSource screwSound;
    public AudioSource collectTokenSound;
    public AudioSource playerDeathSound;
  

    public TextMeshProUGUI tokenText;

    public Animator playerAnim;
    public Animator tokenAnim;
    public Animator ventAnim;

    Vector3 checkPointLocation= new Vector3(0,0,0);

    [Header("GameOver")]
    public GameObject GameOverScreen;
    public GameObject RedScreen;
    public Image blackScreen;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI disText;
    public Animator buttonAnim;
    public AudioSource footStep;
    public CharacterController playerCollider;
    // private IInteractable ventInteractable;

    void Awake()
    {
        LMS = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManagerScript>();
    }
    void Start()
    {
        Vent=GetComponent<VentInteractionScript>();
        GameOverScreen.SetActive(false);
        RedScreen.SetActive(false);
        ventTextUI.SetActive(false);
        screwDriverUI.SetActive(false);
        tokenAnim.keepAnimatorStateOnDisable = true;
        ventAnim.keepAnimatorStateOnDisable = true;
    }
    IEnumerator GameOver()
    {
        if (footStep.isPlaying)
        {
            footStep.Stop();
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerCollider.enabled = false;
        GameOverScreen.SetActive(true);
        RedScreen.SetActive(true);
        playerAnim.SetBool("isDead", true);
        //  playerMovement.speed = 0f;
        LMS.canInput = false;
        yield return new WaitForSeconds(1f);
        BlackScreenFadeIn();
        yield return new WaitForSeconds(1f);
        TextFadeIn();
        yield return new WaitForSeconds(1f);
        desFadeIn();
        buttonAnim.SetBool("isGameOver", true);
        yield return null;
    }

    public void Restart()
    {
        StartCoroutine(RestartProcess());
    }
    IEnumerator RestartProcess()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameObject.transform.position = checkPointLocation;
        //playerCollider.enabled = false;
        buttonAnim.SetBool("isGameOver", false);
        playerAnim.SetBool("isDead", false);
        yield return new WaitForSeconds(1f);
        TextFadeOut();
        desFadeOut();
        yield return new WaitForSeconds(1f);
        RedScreen.SetActive(false);
        yield return new WaitForSeconds(1f);
        //LMS.canInput = true;
        BlackScreenFadeOut();
        yield return new WaitForSeconds(0.5f);
       // playerCollider.enabled = false;
        LMS.canInput = true;
        playerCollider.enabled = true;
        //yield return null;
    }
    private void OnTriggerEnter(Collider collisioninfo)
    {
        if (collisioninfo.CompareTag("Token"))
        {
            collectTokenSound.Play();
            Debug.Log("TokenEarned");
            Destroy(collisioninfo.gameObject);
            carriedTokens++;
            UpdateToken();
            if (collisioninfo.TryGetComponent(out Transform setPoint))
            {
                Vector3 y= setPoint.position;
                y.y = 0f;
                setPoint.position = y;
                checkPointLocation=setPoint.position;
            }
        }
        if (collisioninfo.CompareTag("Enemy"))
        {
            if (playerInsideVent) return;
            playerDeathSound.Play();
            StartCoroutine(GameOver());
            Debug.Log("Player died");
            Animator enemyAnim = collisioninfo.GetComponentInChildren<Animator>();
            if (enemyAnim != null)
            {
                enemyAnim.SetTrigger("isAttack");
            }

        }
        if (collisioninfo.CompareTag("Screwdriver"))
        {
            screwSound.Play();
            Debug.Log("Screwdriver");
         
            Destroy(collisioninfo.gameObject);
            if (!holdingScrewdriver)
            {
                screwDriverUI.SetActive(true);

                holdingScrewdriver = true;
            }
           // carriedTokens++;
        }
        if (collisioninfo.TryGetComponent(out VentEntrance entrance))
        {
            playeratVent = true;
            ventIndex = entrance.VentIndex;
        }

        if (collisioninfo.TryGetComponent(out IInteractable interactable))
        {
            
            if (collisioninfo.CompareTag("CameraMachine"))
            {
                if (!holdingScrewdriver || playerDetected) return;
                SetPrompt(collisioninfo, true);
                currentInteractable = interactable;
            }
            else // includes TokenMachine and everything else
            {
                SetPrompt(collisioninfo, true);
                currentInteractable = interactable;
            }
        }
    }

    private void OnTriggerExit(Collider collisioninfo)
    {
   
        if (collisioninfo.TryGetComponent(out VentEntrance entrance) && entrance.VentIndex == ventIndex)
        {
            playeratVent = false;
            ventIndex = -1;
        }

        if (collisioninfo.TryGetComponent(out IInteractable interactable))
        {
            if (interactable == currentInteractable)
            {
                SetPrompt(collisioninfo, false);
                currentInteractable = null;
            }
        }

     
    }

    private void SetPrompt(Collider c, bool state)
    {
        Animator ui = c.GetComponentInChildren<Animator>();
        if (ui != null) ui.SetBool("isTrigger", state);
    }
    public void hideScrewdriverImage()
    {
        screwDriverUI.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (!LMS.canInput) return;
        if ( Input.GetKeyDown(KeyCode.V))
        {
 
            if (playerInsideVent)
            {
                ventOut.Play();
                Vent.ExitVent();
                StartCoroutine(VentTextUp());
            }
            else if (playeratVent && !PlayerCameraChange.isSwitching)
            {
                if (!cineMachine1st.activeSelf || cineMachine3rd.activeSelf) return;
                ventIn.Play();
                Vent.Process(ventIndex);
                StartCoroutine(VentTextDown());
            }
            return;

        }
        if (playerInsideVent) return;
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
    public void UpdateToken()
    {
        tokenText.text = $"{carriedTokens}";
        tokenAnim.SetTrigger("isInsert");

    }
    IEnumerator VentTextDown()
    {
        ventTextUI.SetActive(true);
        ventAnim.SetBool("isEnter", true);
        yield return null;
    }
    IEnumerator VentTextUp()
    {
        //ventTextUI.SetActive(true);
        ventAnim.SetBool("isEnter", false);
        yield return new WaitForSeconds(0.9f);
        ventTextUI.SetActive(false);

    }



    public void BlackScreenFadeIn()
    {
        StartCoroutine(FadeImage(0f, 1f));
    }

    public void BlackScreenFadeOut()
    {
        StartCoroutine(FadeImage(1f, 0f));
    }
    IEnumerator FadeImage(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            Color c = blackScreen.color;
            c.a = Mathf.Lerp(from, to, t / 1f);
            blackScreen.color = c;
            yield return null;
        }
    }
    public void TextFadeIn()
    {
        StartCoroutine(FadeText(0f, 1f));
    }

    public void TextFadeOut()
    {
        StartCoroutine(FadeText(1f, 0f));
    }

    IEnumerator FadeText(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            Color c = gameOverText.color;
            c.a = Mathf.Lerp(from, to, t / 1f);
            gameOverText.color = c;
            yield return null;
        }
    }
    public void desFadeIn()
    {
        StartCoroutine(desText(0f, 1f));
    }

    public void desFadeOut()
    {
        StartCoroutine(desText(1f, 0f));
    }

    IEnumerator desText(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            Color c = disText.color;
            c.a = Mathf.Lerp(from, to, t / 1f);
            disText.color = c;
            yield return null;
        }
    }
}
//fggf
