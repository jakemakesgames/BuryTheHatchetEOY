using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource m_mainVolume;
    public AudioSource m_sfxVolume;
    public AudioSource m_musicVolume;

    float m_mainVolumeValue;
    float m_sfxVolumeValue;
    float m_musicVolumeValue;


    public float GetMainVolume()
    {
        return m_mainVolumeValue;
    }

    public static SoundManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            //Destroys this object
            Destroy(gameObject);
        }
    }

    public void MainVolume()
    {
        m_mainVolumeValue = m_mainVolume.volume;

        PlayerPrefs.SetFloat("m_mainVolumeValue", m_mainVolumeValue);
    }

    public void SFXVolume(float a_sfxVolume)
    {
        m_sfxVolume.volume = a_sfxVolume;
    }

    public void MusicVolume(float a_musicVolume)
    {
        m_musicVolume.volume = a_musicVolume;
    }

    public void LoadScene()
    {

    }

}
