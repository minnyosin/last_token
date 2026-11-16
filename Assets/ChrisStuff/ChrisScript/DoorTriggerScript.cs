using UnityEngine;
using System.Collections;
public class DoorTriggerScript : MonoBehaviour
{
    public GameObject doorMesh;
    public BoxCollider collider;
    public Animator doorAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider=GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("PlayerEnter");
            StartCoroutine(DoorSlideDown());
        }
    }

    IEnumerator DoorSlideDown()
    {
        collider.enabled = false;
        doorAnim.SetBool("isEnter", true);
        yield return new WaitForSeconds(5f);
        doorAnim.SetBool("isEnter", false);
        yield return new WaitForSeconds(0.6f);
        collider.enabled = true;
    }
}
