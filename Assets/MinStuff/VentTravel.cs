using UnityEngine;

public class VentTravel : MonoBehaviour
{
    public CharacterController controller;

    public Transform[] vents;
    public GameObject ventMenu;
    public GameObject firstPerson;
    public GameObject topDownCamera;
    public float ventTime = 10f;

    bool wasFirstPerson;
    bool wasTopDown;
    float timeleft;

    int currentVentIndex = -1;
    CursorLockMode oldCursorLock;
    bool oldCursorVisible;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentVentIndex == -1 && Input.GetKeyDown(KeyCode.V))
        {
            for (int i = 0; i < vents.Length; i++)
            {
                if (!IsNear(vents[i]))
                {
                    continue;
                }
                currentVentIndex = i;

                wasFirstPerson = firstPerson.activeSelf;
                wasTopDown = topDownCamera.activeSelf;
                firstPerson.SetActive(false);
                topDownCamera.SetActive(true);

                timeleft = ventTime;
                oldCursorLock = Cursor.lockState;
                oldCursorVisible = Cursor.visible;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                ventMenu.SetActive(true);
                break;
            }
        }
        if (currentVentIndex >= 0)
        {
            timeleft -= Time.deltaTime;
            if (timeleft <= 0 || !IsNear(vents[currentVentIndex]))
            {
                ExitVent();
            }
        }
    }

    public void NextVent()
    {
        if (currentVentIndex == -1 || vents.Length < 2)
        {
            return;
        }
        int next = currentVentIndex + 1;
        if (next == vents.Length)
        {
            next = 0;
        }
        TravelTo(next);
    }

    public void PreviousVent()
    {
        if ((currentVentIndex == -1 || vents.Length < 2))
        {
            return;
        }
        int previous = currentVentIndex - 1;
        if (previous < 0)
        {
            previous = vents.Length - 1;

        }
        TravelTo(previous);
    }

    public void ExitVent()
    {
        if (currentVentIndex == -1)
        {
            return;
        }
        currentVentIndex = -1;
        ventMenu.SetActive(false);
        topDownCamera.SetActive(wasTopDown);
        firstPerson.SetActive(wasFirstPerson);
        Cursor.lockState = oldCursorLock;
        Cursor.visible = oldCursorVisible;
    }
    void TravelTo(int ventIndex)
    {
        controller.enabled = false;
        transform.position = vents[ventIndex].position;
        controller.enabled = true;

        currentVentIndex = ventIndex;
        timeleft = ventTime;
    }
    bool IsNear(Transform vent)
    {
        Vector3 position = vent.position;
        position.y = transform.position.y;
        return Vector3.Distance(transform.position, position) < 1.5f;
    }
}
