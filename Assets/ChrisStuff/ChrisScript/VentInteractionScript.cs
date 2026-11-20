using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
public class VentInteractionScript : MonoBehaviour
{

    [System.Serializable]
    public class Vent
    {
        public Transform entrance;
        public Transform cameraPoint;
        public Transform exitPoint;
    }

    public Vent[] vents;
    public CharacterController controller;
    public Transform player;
    public PlayerMovement movement;
    public MouseLook mouseLook;
    public PlayerCameraChangeScript cameraSwitch;
    public SpriteRenderer playerSprite;
    public PlayerInteractor playerInteractor;

    public GameObject mainCamera;
    public GameObject ventCamera;
    public GameObject cineMachine1st;
    public GameObject cineMachine3rd;
    public GameObject tokenUI;
    public GameObject inventoryUI;
    public GameObject eyeUI;
    //public GameObject ventTextUI;
    public Animator eyeAnim;
    public AudioSource footstep;
    public AudioSource ventClimbAudio;

    public float ventLookSpeed = 2f;
    bool canInput=true;



   // bool insideVent;
    int currentVent;
    float leftRight;
    float upDown;
    float startingDirection;
    bool inventoryWasVisible;
    // Start is called once before the first execution of Update after the MonoBehaviour is created




    void Start()
    {
      //  ventTextUI.SetActive(false);
        eyeUI.SetActive(false);
        ventCamera.SetActive(false);
        eyeAnim.keepAnimatorStateOnDisable = true;
    }

    void Update()
    {
        if (playerInteractor.playerInsideVent)
        {
            LookAround();
            if (!canInput) return;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(ChangeVent(0));
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(ChangeVent(1));
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                StartCoroutine(ChangeVent(2));
            }
         
        }

    }
public void Process(int index)
    {
        if (index < 0 || index >= vents.Length) return;
        Debug.Log("isworkinghere");
        
        if (playerInteractor.playerInsideVent || !canInput)
        {
            return;
        }
        EnterVent(index);
}
    void EnterVent(int number)
    {
        if (footstep.isPlaying)
        {
            footstep.Stop();
        }
        playerInteractor.playerInsideVent = true;

       // ventTextUI.SetActive(true);
        tokenUI.SetActive(false);
        inventoryWasVisible = inventoryUI.activeSelf;
        inventoryUI.SetActive(false);
        movement.enabled = false;
        mouseLook.enabled = false;
        cameraSwitch.enabled = false;
        controller.enabled = false;
        playerSprite.enabled = false;

        mainCamera.SetActive(false);
        ventCamera.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentVent = -1;
        FirstChangeVent(number);
    }

    void FirstChangeVent(int number)
    {
        if (number < 0 || number >= vents.Length)
        {
            return;
        }
        if (number == currentVent)
        {
            return;
        }
        currentVent = number;
        Transform point = vents[number].cameraPoint;
        ventCamera.transform.position = point.position;

        leftRight = point.eulerAngles.y;
        startingDirection = leftRight;
        upDown = point.eulerAngles.x;
        if (upDown > 180)
        {
            upDown -= 360;
        }

        ventCamera.transform.rotation = Quaternion.Euler(upDown, leftRight, 0);
    }
   
    IEnumerator ChangeVent(int number)
    {
        
        if (number < 0 || number >= vents.Length)
        {
            yield break;
        }
        if (!canInput || !playerInteractor.playerInsideVent || number == currentVent)
        {
            yield break;
        }
        canInput = false;
        currentVent = number;
        eyeUI.SetActive(true);
        eyeAnim.SetBool("isClose", true);
        ventClimbAudio.Play();
        yield return new WaitForSeconds(0.4f);
        eyeAnim.SetBool("isClose", false);
        ventClimbAudio.Stop();
        Transform point = vents[number].cameraPoint;
        ventCamera.transform.position = point.position;

        leftRight = point.eulerAngles.y;
        startingDirection = leftRight;
        upDown = point.eulerAngles.x;
        if (upDown > 180)
        {
            upDown -= 360;
        }
        ventCamera.transform.rotation = Quaternion.Euler(upDown, leftRight, 0);
        yield return new WaitForSeconds(0.4f);
       // ventClimbAudio.Stop();
        eyeUI.SetActive(false);
        canInput = true;
    }

    void LookAround()
    {
        leftRight += Input.GetAxis("Mouse X") * ventLookSpeed;
        upDown -= Input.GetAxis("Mouse Y") * ventLookSpeed;

        leftRight = Mathf.Clamp(leftRight, startingDirection - 70, startingDirection + 70);
        upDown = Mathf.Clamp(upDown, -60, 60);

        ventCamera.transform.rotation = Quaternion.Euler(upDown, leftRight, 0);
    }

  public  void ExitVent()
    {
        if (!playerInteractor.playerInsideVent || currentVent < 0 || currentVent >= vents.Length)
        {
            return;
        }
        StopAllCoroutines();
        canInput = true;
        ventClimbAudio.Stop();
        eyeAnim.SetBool("isClose", false);
        eyeUI.SetActive(false);
        Vector3 actualPos = vents[currentVent].exitPoint.position;
        player.transform.position = actualPos;

        mouseLook.SetLook(leftRight, upDown);

        //ventTextUI.SetActive(false);
        tokenUI.SetActive(true);
        inventoryUI.SetActive(inventoryWasVisible);
        mainCamera.SetActive(true);
        ventCamera.SetActive(false);
        cineMachine1st.SetActive(true);
        cineMachine3rd.SetActive(false);

        playerInteractor.ClearVentTrigger();
        controller.enabled = true;
        movement.enabled = true;
        mouseLook.enabled = true;
        cameraSwitch.enabled = true;
        playerSprite.enabled = true;
        playerInteractor.playerInsideVent = false;
    }
}
