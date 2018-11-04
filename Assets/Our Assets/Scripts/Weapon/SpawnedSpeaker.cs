using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 22/08/2018
//Last edited 05/11/2018

[RequireComponent(typeof(AudioSource))]
public class SpawnedSpeaker : MonoBehaviour {
    
    private AudioSource m_audioSource;
    private bool m_soundStarted = false;
    private SoundManager m_soundManager;
    public float SFXVolume {
        get {
            if (m_soundManager == null) return 1f;
            else { return m_soundManager.MasterVolume * m_soundManager.SFXVolume; }
        }
    }

    public AudioSource AudioSource {
        get { return m_audioSource; } 
        set { m_audioSource = value; }
    }

    private void Awake() {
        AudioSource = GetComponent<AudioSource>();
        m_soundManager = FindObjectOfType<SoundManager>();
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
            else
                AudioSource.volume = SFXVolume;
        }
    }
}
