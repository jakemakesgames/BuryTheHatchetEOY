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
    [SerializeField] float m_combatTime;

    #endregion

    bool m_musicTransition;
    bool m_combatStatePrevFrame = false;
    bool m_countingDown;

    float m_inCombatVolume;
    float m_outOfCombatVolume;

    float m_combatTimer;

    PlayerInput m_playerInput;

    private void Start()
    {
        m_playerInput = FindObjectOfType<PlayerInput>();

        m_outOfCombatMusic.Play();
        m_inCombatMusic.Play();

        m_musicTransition = m_playerInput.InCombat;

        m_inCombatVolume = 0;
        m_outOfCombatVolume = 1;

        m_inCombatMusic.volume = m_inCombatVolume;
        m_outOfCombatMusic.volume = m_outOfCombatVolume;
    }

    private void Update()
    {
        FinishedCombat();
        TransitionBetween(m_musicTransition);
    }

    void TransitionBetween(bool a_toCombat)
    {
        //In Combat
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
        //Out of combat
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

    void FinishedCombat()
    {
        if (!m_playerInput.InCombat && m_combatStatePrevFrame)
        {
            m_countingDown = true;
        }

        //Out of combat
        if (m_countingDown)
        {
            if (!m_playerInput.InCombat)
            {
                m_combatTimer -= Time.deltaTime;

                if (m_combatTimer <= 0)
                {
                    m_musicTransition = false;
                    m_countingDown = false;
                    m_combatTimer = m_combatTime;
                }
            }
            else
            {
                m_countingDown = false;
                m_combatTimer = m_combatTime;
            }
        }
        //In combat
        else if(m_playerInput.InCombat)
        {
            m_musicTransition = true;
            m_countingDown = false;
        }

        m_combatStatePrevFrame = m_playerInput.InCombat;
    }
}
