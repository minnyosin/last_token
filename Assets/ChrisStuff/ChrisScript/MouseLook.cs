using UnityEngine;

public class MouseLook : MonoBehaviour


{

    public float mouseSensitivity = 100f;
    public Transform playerBody;
    float xRotation = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void SetLook(float leftRightAngle, float upDownAngle)
    {
        xRotation = upDownAngle;
        playerBody.rotation = Quaternion.Euler(0, leftRightAngle, 0);
        transform.localRotation = Quaternion.Euler(upDownAngle, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

    }
}
