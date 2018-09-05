using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 22/08/2018
//Last edited 22/08/2018

[RequireComponent(typeof(AudioSource))]
public class SpawnedSpeaker : MonoBehaviour {
    
    private AudioSource m_audioSource;
    private bool m_soundStarted = false;

    public AudioSource AudioSource {
        get { return m_audioSource; } 
        set { m_audioSource = value; }
    }

    private void Awake() {
        AudioSource = GetComponent<AudioSource>();
    }

    //When the sound finishes playing destroy this object
    private void Update() {
        if (m_soundStarted == false) {
            if (AudioSource.isPlaying)
                m_soundStarted = true;
        }
        else {
            if (AudioSource.isPlaying == false)
                Destroy(gameObject);
        }

    }
}
