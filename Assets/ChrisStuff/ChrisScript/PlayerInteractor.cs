using UnityEngine;

public class PlayerInteractor: MonoBehaviour
{
    private IInteractable currentInteractable;


    private void OnTriggerEnter(Collider collisioninfo)
    {
        if (collisioninfo.TryGetComponent(out IInteractable interactable))
        {
            Animator UI = collisioninfo.GetComponentInChildren<Animator>();
            UI.SetBool("isTrigger", true);
            currentInteractable = interactable;
            Debug.Log("Enter");
        }
    }

    private void OnTriggerExit(Collider collisioninfo)
    {
        if (collisioninfo.TryGetComponent(out IInteractable interactable) && interactable == currentInteractable)
        {
            Animator UI = collisioninfo.GetComponentInChildren<Animator>();
            UI.SetBool("isTrigger", false);
            currentInteractable = null;
            Debug.Log("Left");
        }
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
