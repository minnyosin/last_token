/*using UnityEngine;

public class BillBoardScript : MonoBehaviour
{
    //public Camera mainCamera;
    //Camera ventCamera;
    [SerializeField] public bool freezeXZAxis = false;
    private void Update()
    {
        if (Camera.main == null) return;
        if (freezeXZAxis)
        {
            transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }
        //  transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
    void Start()
    {

    }

    // Update is called once per frame

}*/
using UnityEngine;

public class BillBoardScript : MonoBehaviour
{
    public GameObject Main;
    public GameObject Vent;

    public Camera mainCamera;
    public Camera ventCamera;
    [SerializeField] public bool freezeXZAxis = false;
    void Awake()
    {

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        ventCamera = GameObject.FindGameObjectWithTag("VentCamera").GetComponent<Camera>();

        Main = GameObject.FindGameObjectWithTag("MainCamera");
        Vent = GameObject.FindGameObjectWithTag("VentCamera");
    }
    private void Update()
    {
        //if (Camera.main == null) return;
        if (freezeXZAxis)
        {
            if (Main.activeSelf&&!Vent.activeSelf)
            {
                transform.rotation = Quaternion.Euler(0f, mainCamera.transform.rotation.eulerAngles.y, 0f);
            }
            else if (!Main.activeSelf && Vent.activeSelf)
            {
                transform.rotation = Quaternion.Euler(0f, ventCamera.transform.rotation.eulerAngles.y, 0f);
            }
            // transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            if (Main.activeSelf && !Vent.activeSelf)
            {
               // Debug.Log("hey");
                transform.rotation = mainCamera.transform.rotation;
            }
            else if (!Main.activeSelf && Vent.activeSelf)
            {
                //Debug.Log("hello");
                transform.rotation = ventCamera.transform.rotation;
            }
            // transform.rotation = Camera.main.transform.rotation;
        }
        //  transform.rotation = Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f);
    }
    void Start()
    {

    }

    // Update is called once per frame

}
