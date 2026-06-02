using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
     public Light mylight;
     public AudioSource danger;
     public AudioSource loudMusic;
     public AudioSource softMusic;
    // Start is called before the first frame update
    public float duration = 50.0F;
    public float fadeTime = 1.0F;
    public ParticleSystem ps;
    void Start()
    {
       var em = ps.emission;
         em.enabled = false;

        softMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Danger Zone!");
        //we play the sound
        danger.Play();
        softMusic.volume = Mathf.Lerp(softMusic.volume, 0.0F, fadeTime);
        loudMusic.Play();
        var em = ps.emission;
        em.enabled = true;
    }
    void OnTriggerStay(Collider other)
    {
        Debug.Log("Within Danger Zone!");
        //we recursively dim the light
        //when within the danger zone
        mylight.intensity = Mathf.PingPong(Time.time, 1);
        //we define the time within which the color will change
        float t = Mathf.PingPong(Time.time, duration) / duration;
        //we change the color
        mylight.color = Color.Lerp(Color.red, Color.gray, t);
        
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("Out of Danger!");
        mylight.intensity = 1;
        mylight.color = Color.white;
        //we stop the sound
        danger.Stop();
        // we fade the music back in
        softMusic.volume = Mathf.Lerp(softMusic.volume, 0.25F, fadeTime);
        loudMusic.Stop();
        var em = ps.emission;
        em.enabled = false;

    }
    
}
