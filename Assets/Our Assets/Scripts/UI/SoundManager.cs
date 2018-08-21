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

    public AudioSource m_buttonSound;
    public AudioSource m_backSound;
    public AudioSource m_musicSound;

    #endregion

    #region Private Regions

    private float m_masterVolume;

    #endregion

    #region Saving Instance

    public static SoundManager m_instance;

    //Creates a Singleton
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

    //Sound Effect Slider Controls
    public void SFXVolume()
    {
        m_buttonSound.volume = m_SFXSlider.value * m_masterSlider.value;
        m_backSound.volume = m_SFXSlider.value * m_masterSlider.value;
    }

    //Music Slider Controls
    public void MusicVolume()
    {
        m_musicSound.volume = m_musicSlider.value * m_masterSlider.value;
    }

    //Button Play Sound
    public void ButtonSound()
    {
        m_buttonSound.Play();
    }

    //Escape Play Sound
    public void BackSound()
    {
        m_backSound.Play();
    }

    #endregion

}
