using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class CameraMachineScript : MonoBehaviour, IInteractable
{
    public List<GameObject> camera;
    public GameObject collider;
    public GameObject p;
    public PlayerInteractor player;
    public AudioSource electricAudio;
    public AudioSource bangAudio;
    public GameObject effect;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        p = GameObject.FindGameObjectWithTag("Player");
        player=p.GetComponent<PlayerInteractor>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact()
    {
        if (!player.holdingScrewdriver||player.playerDetected)
        {
            Debug.Log( "Cannot interact yet");
            return;
        }
        StartCoroutine(DisablingCamera());
        
    }
    IEnumerator DisablingCamera()
    {
        Instantiate(effect, transform.position, Quaternion.identity);
        player.hideScrewdriverImage();
        player.holdingScrewdriver = false;
        //collider.SetActive(false);
        electricAudio.Play();
        bangAudio.Play();
        for (int i = 0; i < camera.Count; i++)
        {
            Animator camAnim = camera[i].GetComponent<Animator>();
            camAnim.enabled = false;

           // SecurityLightBlinking light= camera[i].GetComponent<SecurityLightBlinking>();
           // light.enabled = false;

            Transform Lights = camera[i].transform.Find("Lights");
            Lights.gameObject.SetActive(false);

            SecurityLightBlinking camDisable = camera[i].GetComponent<SecurityLightBlinking>();
            camDisable.DisableCamera(camera[i]);
           // StartCoroutine(CameraDisableRotation(camera[i]));

            yield return null;
        }
       // yield return new WaitForSeconds(0.2f);
        collider.SetActive(false);
    }

    /*IEnumerator CameraDisableRotation(GameObject cam)
    {
        float duration = 1f;

     float rotz=cam.transform.localEulerAngles.z;
        while(duration > 0f)
        {
            rotz = Mathf.Lerp(9.03f, rotz, duration / 1);
            Vector3 rot= cam.transform.localEulerAngles;
            rot.z = rotz;
            cam.transform.localEulerAngles=rot;
            duration -= Time.deltaTime;
            yield return null;
        }
     


    }*/
}
