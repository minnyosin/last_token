using UnityEngine;

public class VentTravel : MonoBehaviour
{
    public CharacterController controller;

    public Transform[] vents;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TravelTo(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TravelTo(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TravelTo(2);
        }
    }
    void TravelTo(int index)
    {
        if (controller == null || vents == null || index >= vents.Length)
        {
            return;
        }
            

        Transform destination = vents[index];
        if (destination == null || IsNear(destination))
        {
            return;
        }
            

        foreach (Transform vent in vents)
        {
            if (vent == null || !IsNear(vent))
            {
                continue;
            }

            controller.enabled = false;
            transform.position = destination.position;
            controller.enabled = true;
            return;
        }
    }
    bool IsNear(Transform vent)
    {
        Vector3 position = vent.position;
        position.y = transform.position.y;
        return Vector3.Distance(transform.position, position) < 1.5f;
    }
}
