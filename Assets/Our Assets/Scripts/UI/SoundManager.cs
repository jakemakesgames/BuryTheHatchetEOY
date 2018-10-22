using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Public Variables
    [Header("Sliders")]
    //[SerializeField] Slider m_mainSlider;
    //[SerializeField] Slider m_soundEffectsSlider;
    //[SerializeField] Slider m_musicSlider;

    [Header("AudioSources")]
    [SerializeField] AudioSource m_outOfCombatMusic;
    //[SerializeField] AudioSource m_transitionIntoCombatMusic;
    [SerializeField] AudioSource m_inCombatMusic;
    [Header("Volume Fade Speed")]
    [Tooltip("Fade in speed")]
    [SerializeField] float m_fadeInControl;
    [Tooltip("Fade out speed")]
    [SerializeField] float m_fadeOutControl;

    public bool m_musicTransition = false;


    #endregion

    float m_inCombatVolume;
    float m_outOfCombatVolume;

    PlayerInput m_playerInput;

    private void Start()
    {
        m_playerInput = FindObjectOfType<PlayerInput>();

        m_outOfCombatMusic.Play();
        m_inCombatMusic.Play();

        m_inCombatVolume = 0;
        m_outOfCombatVolume = 1;

        m_inCombatMusic.volume = m_inCombatVolume;
        m_outOfCombatMusic.volume = m_outOfCombatVolume;
    }

    private void Update()
    {
        TransitionBetween(m_musicTransition);
    }

    void TransitionBetween(bool a_toCombat)
    {
        if (a_toCombat)
        {
            if (m_outOfCombatVolume > 0)
            {
                m_outOfCombatVolume -= Time.deltaTime * m_fadeOutControl;
            }
            else
            {
                m_outOfCombatVolume = 0;
            }

            if (m_inCombatVolume < 1)
            {
                m_inCombatVolume += Time.deltaTime * m_fadeInControl;
            }
            else
            {
                m_inCombatVolume = 1;
            }
        }
        else
        {
            if (m_outOfCombatVolume < 1)
            {
                m_outOfCombatVolume += Time.deltaTime * m_fadeOutControl;
            }
            else
            {
                m_outOfCombatVolume = 1;
            }

            if (m_inCombatVolume > 0)
            {
                m_inCombatVolume -= Time.deltaTime * m_fadeInControl;
            }
            else
            {
                m_inCombatVolume = 0;
            }
        }
        m_inCombatMusic.volume = m_inCombatVolume;
        m_outOfCombatMusic.volume = m_outOfCombatVolume;
    }
}
