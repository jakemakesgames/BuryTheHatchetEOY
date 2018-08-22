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

    private void Start() {
        m_audioSource = GetComponent<AudioSource>();
    }

    //When the sound finishes playing destroy this object
    private void Update() {
        if (m_soundStarted == false) {
            if (m_audioSource.isPlaying)
                m_soundStarted = true;
        }
        else {
            if (m_audioSource.isPlaying == false)
                Destroy(gameObject);
        }

    }
}
