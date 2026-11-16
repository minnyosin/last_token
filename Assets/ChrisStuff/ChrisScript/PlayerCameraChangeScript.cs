using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraChangeScript : MonoBehaviour
{
    public Transform player;
    public GameObject cineMachine1st;
    public GameObject cineMachine3rd;
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

        if(cineMachine1st.activeSelf&& !cineMachine3rd.activeSelf)
        {
            cineMachine1st.SetActive(false);
            cineMachine3rd.SetActive(true);
            Vector3 e = player.eulerAngles;
            e.y = 0f;
            player.eulerAngles = e;

        }
        else if (!cineMachine1st.activeSelf && cineMachine3rd.activeSelf)
        {
            cineMachine1st.SetActive(true);
            cineMachine3rd.SetActive(false);

        }
    }
}
