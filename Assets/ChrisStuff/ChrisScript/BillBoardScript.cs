using UnityEngine;

public class BillBoardScript : MonoBehaviour
{
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

}
