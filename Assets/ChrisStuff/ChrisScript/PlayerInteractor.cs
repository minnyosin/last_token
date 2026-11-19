using UnityEngine;

public class PlayerInteractor: MonoBehaviour
{
    public GameObject cineMachine1st;
    public GameObject cineMachine3rd;

    public int carriedTokens;
    public bool holdingScrewdriver=false;
    public bool playerDetected = false;
    public bool playeratVent = false;
    public bool playerInsideVent = false;
    public GameObject screwDriverUI;
    private IInteractable currentInteractable;
    public VentInteractionScript Vent;
    public PlayerCameraChangeScript PlayerCameraChange;
    int ventIndex=-1;
    public AudioSource ventIn;
    public AudioSource ventOut;
    public AudioSource screwSound;
    // private IInteractable ventInteractable;


    void Start()
    {
        Vent=GetComponent<VentInteractionScript>();
        screwDriverUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider collisioninfo)
    {
        if (collisioninfo.CompareTag("Token"))
        {
            Debug.Log("TokenEarned");
            Destroy(collisioninfo.gameObject);
            carriedTokens++;
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
     
        if ( Input.GetKeyDown(KeyCode.V))
        {
 
            if (playerInsideVent)
            {
                ventOut.Play();
                Vent.ExitVent();
            }
            else if (playeratVent && !PlayerCameraChange.isSwitching)
            {
                if (!cineMachine1st.activeSelf || cineMachine3rd.activeSelf) return;
                ventIn.Play();
                Vent.Process(ventIndex);
            }
            return;

        }
        if (playerInsideVent) return;
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
//fggf
