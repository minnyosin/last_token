using UnityEngine;

public class VentTravel : MonoBehaviour
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
    public PlayerMovement movement;
    public MouseLook mouseLook;
    public PlayerCameraChangeScript cameraSwitch;
    public SpriteRenderer playerSprite;

    public Camera mainCamera;
    public Camera ventCamera;
    public GameObject cineMachine1st;
    public GameObject cineMachine3rd;

    public float enterDistance = 1.5f;
    public float ventLookSpeed = 2f;

    bool insideVent;
    int currentVent;
    float leftRight;
    float upDown;
    float startingDirection;

    void Start()
    {
        ventCamera.enabled = false;
    }

    void Update()
    {
        if (insideVent)
        {
            LookAround();

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeVent(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeVent(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeVent(2);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                ExitVent();
            }
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (!cineMachine1st.activeInHierarchy || cineMachine3rd.activeInHierarchy)
            {
                return;
            }

            for (int i= 0; i < vents.Length; i++)
            {
                Vector3 ventPosition = vents[i].entrance.position;
                ventPosition.y = controller.transform.position.y;

                float distance = Vector3.Distance(controller.transform.position, ventPosition);
                if (distance < enterDistance)
                {
                    EnterVent(i);
                    break;
                }
            }
        }
    }

    void EnterVent(int number)
    {
        insideVent = true;

        movement.enabled = false;
        mouseLook.enabled = false;
        cameraSwitch.enabled = false;
        controller.enabled = false;
        playerSprite.enabled = false;

        mainCamera.enabled = false;
        ventCamera.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentVent = -1;
        ChangeVent(number);
    }

    void ChangeVent(int number)
    {
        if (number >= vents.Length)
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
        if (upDown >180)
        {
            upDown -= 360;
        }

        ventCamera.transform.rotation = Quaternion.Euler(upDown, leftRight, 0);
    }

    void LookAround()
    {
        leftRight += Input.GetAxis("Mouse X") * ventLookSpeed;
        upDown -= Input.GetAxis("Mouse Y") * ventLookSpeed;

        leftRight = Mathf.Clamp(leftRight, startingDirection - 70, startingDirection + 70);
        upDown = Mathf.Clamp(upDown, -60, 60);

        ventCamera.transform.rotation = Quaternion.Euler(upDown, leftRight, 0);
    }

    void ExitVent()
    {
        controller.transform.position = vents[currentVent].exitPoint.position;

        mouseLook.SetLook(leftRight, upDown);

        ventCamera.enabled = false;
        mainCamera.enabled = true;
        cineMachine1st.SetActive(true);
        cineMachine3rd.SetActive(false);

        controller.enabled = true;
        movement.enabled = true;
        mouseLook.enabled = true;
        cameraSwitch.enabled = true;
        playerSprite.enabled = true;
        insideVent = false;
    }
}
