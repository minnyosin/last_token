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
}
