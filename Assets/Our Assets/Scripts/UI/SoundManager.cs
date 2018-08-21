using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Public Variables

    public Slider m_masterSlider;
    public Slider m_musicSlider;
    public Slider m_SFXSlider;

    public AudioSource m_sfxSound;
    public AudioSource m_musicSound;

    #endregion

    #region Private Regions

    private float m_masterVolume;

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
        m_musicSound.Play();
        m_musicSound.loop = true;
    }

    private void Update()
    {
        MusicVolume();
        SFXVolume();
    }

    #endregion

    #region Public Functions

    public float GetMainVolume()
    {
        return m_masterSlider.value;
    }

    public void SFXVolume()
    {
        m_sfxSound.volume = m_SFXSlider.value * m_masterSlider.value;
    }

    public void MusicVolume()
    {
        m_musicSound.volume = m_musicSlider.value * m_masterSlider.value;
    }

    public void ButtonSound()
    {
        m_sfxSound.Play();
    }

    #endregion

}
