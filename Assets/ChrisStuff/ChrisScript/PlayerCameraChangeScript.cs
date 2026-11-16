using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraChangeScript : MonoBehaviour
{
    public GameObject cineMachine;
    [SerializeField] private InputActionReference interactionReference;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        interactionReference.action.Enable();
        interactionReference.action.started += CameraChanged;
    }

    private void OnDisable()
    {
        interactionReference.action.started -= CameraChanged;
        interactionReference.action.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void CameraChanged(InputAction.CallbackContext context)
    {

        if(cineMachine.activeSelf)
        {
            cineMachine.SetActive(false);
        }
        else
        {
            cineMachine.SetActive(true);
        }
    }
}
