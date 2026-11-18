using UnityEngine;

public class PlayerInteractor: MonoBehaviour
{
    public int carriedTokens;
    public bool holdingScrewdriver=false;
    public bool playerDetected = false;
    public GameObject screwDriverUI;
    private IInteractable currentInteractable;

 
    void Start()
    {
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
            Debug.Log("Screwdriver");
         
            Destroy(collisioninfo.gameObject);
            if (!holdingScrewdriver)
            {
                screwDriverUI.SetActive(true);

                holdingScrewdriver = true;
            }
           // carriedTokens++;
        }
        if (collisioninfo.TryGetComponent(out IInteractable interactable))
        {
            if (collisioninfo.CompareTag("TokenMachine"))
            {
                Animator UI = collisioninfo.GetComponentInChildren<Animator>();
                UI.SetBool("isTrigger", true);
                currentInteractable = interactable;
                Debug.Log("Is a token Machine");
            }
            if (collisioninfo.CompareTag("CameraMachine"))
            {
                if (!holdingScrewdriver || playerDetected) return;
                Animator UI = collisioninfo.GetComponentInChildren<Animator>();
                UI.SetBool("isTrigger", true);
                currentInteractable = interactable;
                Debug.Log("Is a camera Machine");
            }
            else
            {
                Animator UI = collisioninfo.GetComponentInChildren<Animator>();
                UI.SetBool("isTrigger", true);
                currentInteractable = interactable;
                Debug.Log("Enter");
            }
      
        }
    }

    private void OnTriggerExit(Collider collisioninfo)
    {
        if (collisioninfo.TryGetComponent(out IInteractable interactable) && interactable == currentInteractable)
        {
            if (collisioninfo.CompareTag("TokenMachine"))
            {
                Animator UI = collisioninfo.GetComponentInChildren<Animator>();
                UI.SetBool("isTrigger", false);
                currentInteractable = null;
                Debug.Log("Left token Machine");
            }
            if (collisioninfo.CompareTag("CameraMachine"))
            {
               
                Animator UI = collisioninfo.GetComponentInChildren<Animator>();
                UI.SetBool("isTrigger", false);
                currentInteractable = null;
                Debug.Log("Left Camera Machine");
            }
            else
            {
                Animator UI = collisioninfo.GetComponentInChildren<Animator>();
                UI.SetBool("isTrigger", false);
                currentInteractable = null;
                Debug.Log("Left");
            }

        }
    }
    public void hideScrewdriverImage()
    {
        screwDriverUI.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.Interact();
        }
    }
}
//fggf
