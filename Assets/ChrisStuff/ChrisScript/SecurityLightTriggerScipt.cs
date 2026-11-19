using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;


//using NUnit.Framework;
public class SecurityLightTriggerScipt : MonoBehaviour
{
    public List<GameObject> doors;
   // public GameObject doorMesh;
    // public BoxCollider collider;
    public MeshCollider collider;
   // public Animator doorAnim;
    public Animator securityAnim;
    public AudioSource alarmAudio;
    public AudioSource doorOpenAudio;
    public AudioSource doorCloseAudio;

    public GameObject p;
    public PlayerInteractor player;

    public GameObject NVM;
    public NavMeshSurface nav;
    //public AudioSource alarmAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        NVM= GameObject.FindGameObjectWithTag("NavMesh");
        nav=NVM.GetComponent<NavMeshSurface>();
        p = GameObject.FindGameObjectWithTag("Player");
        player=p.GetComponent<PlayerInteractor>();
    }
    void Start()
    {
       securityAnim.keepAnimatorStateOnDisable = true;
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
        player.playerDetected = true;
        alarmAudio.Play();
        doorCloseAudio.Play();
        securityAnim.enabled = false;
        collider.enabled = false;
       // AudioSource.PlayClipAtPoint(doorCloseAudio,doors[0].transform.position);
        for (int i = 0; i < doors.Count; i++)
        {
            Animator doorsAnim= doors[i].GetComponentInChildren<Animator>();
            doorsAnim.SetBool("isEnter", true);
        }
        //doorAnim.SetBool("isEnter", true);
        
        yield return new WaitForSeconds(0.6f);
        nav.BuildNavMesh();
        yield return new WaitForSeconds(5f);
        doorOpenAudio.Play();   
        //AudioSource.PlayClipAtPoint(doorOpenAudio, doors[0].transform.position);
        for (int i = 0; i < doors.Count; i++)
        {
            Animator doorsAnim = doors[i].GetComponentInChildren<Animator>();
            doorsAnim.SetBool("isEnter", false);
        }
        // doorAnim.SetBool("isEnter", false);
        yield return new WaitForSeconds(0.6f);
        nav.BuildNavMesh();
        securityAnim.enabled = true;
        collider.enabled = true;
        player.playerDetected = false;
    }
}
