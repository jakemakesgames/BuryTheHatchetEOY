using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Public Variables

    public Slider m_mainSlider;
    public Slider m_musicSlider;
    public Slider m_SFXSlider;

    public AudioSource m_mainVolume;
    public AudioSource m_sfxVolume;
    public AudioSource m_musicVolume;

    #endregion

    #region Private Regions

    #endregion


    #region Saving Instance

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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        m_mainVolume.Play();
    }

    private void Update()
    {
        MainVolume();
    }

    #endregion

    #region Public Functions

    public float GetMainVolume()
    {
        return m_mainSlider.value;
    }

    public void MainVolume()
    {
        m_mainVolume.volume = m_mainSlider.value;

        PlayerPrefs.SetFloat("m_mainVolumeValue", m_mainSlider.value);
        
    }

    public void SFXVolume()
    {
        m_sfxVolume.volume = m_SFXSlider.value;
    }

    public void MusicVolume()
    {
        m_musicVolume.volume = m_musicSlider.value;
    }

    #endregion

}
