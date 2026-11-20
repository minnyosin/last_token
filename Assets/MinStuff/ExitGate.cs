using UnityEngine;

public class ExitGate : MonoBehaviour
{
    public TestTokenMachine machine;
    Vector3 closedPosition;
    bool open;

    void Start()
    {
        closedPosition = transform.position;
    }

    void Update()
    {
        if (machine == null || !machine.ExitReady) { return; }
        if (!open)
        {
            open = true;
            GetComponent<Collider>().enabled = false;
        }
        transform.position = Vector3.MoveTowards(transform.position,
            closedPosition + Vector3.right * 8f,8f * Time.deltaTime);
    }
}
