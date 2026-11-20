using UnityEngine;

public class RoomDoor : MonoBehaviour
{
    public Transform panel;
    public Transform player;
    public Transform[] enemies;
    public float slideDistance = 7.2f;
    Vector3 closedPosition;

    void Start()
    {
        closedPosition = panel.localPosition;
    }

    void Update()
    {
        bool open = player != null && Vector3.Distance(player.position,transform.position) < 7f;
        foreach (var enemy in enemies)
        {
            if (enemy != null && Vector3.Distance(enemy.position,transform.position) < 7f)
            {
                open = true;
            }
        }
        Vector3 target = closedPosition;
        if (open)
        {
            target.x += slideDistance;
        }
        panel.localPosition = Vector3.MoveTowards(panel.localPosition,target,12f * Time.deltaTime);
    }
}
