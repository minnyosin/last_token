using UnityEngine;
using System.Collections;
public class SecurityLightBlinking : MonoBehaviour
{
    public GameObject blinkingLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Blinking());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DisableCamera(GameObject cam)
    {
        StartCoroutine(CameraDisableRotation(cam));
    }
    IEnumerator Blinking()
    {
       if (blinkingLight.activeSelf)
        {
            yield return new WaitForSeconds(0.5f);
            blinkingLight.SetActive(false);
        }
        //blinkingLight.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        blinkingLight.SetActive(true);
        StartCoroutine(Blinking());
    }
    IEnumerator CameraDisableRotation(GameObject cam)
    {
        float duration = 1f;

        float rotz = cam.transform.localEulerAngles.z;
        while (duration > 0f)
        {
            rotz = Mathf.Lerp(9.03f, rotz, duration / 1);
            Vector3 rot = cam.transform.localEulerAngles;
            rot.z = rotz;
            cam.transform.localEulerAngles = rot;
            duration -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        this.enabled = false;
        blinkingLight.SetActive(false);
    }
}
