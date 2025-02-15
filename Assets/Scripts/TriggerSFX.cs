using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSFX : MonoBehaviour
{
    public AudioSource playsound;

    void OnTriggerEnter(Collider other)
    {
        playsound.Play();
    }
}
