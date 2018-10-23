using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    #region Public Variables
    [Header("Sliders")]
    //[SerializeField] Slider m_mainSlider;
    //[SerializeField] Slider m_soundEffectsSlider;
    //[SerializeField] Slider m_musicSlider;

    [Header("AudioSources")]
    [SerializeField] AudioSource m_outOfCombatMusic;
    [SerializeField] AudioSource m_inCombatMusic;
    [SerializeField] AudioSource m_mainMenuMusic;
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
    UIManager m_UIManager;
    private static SoundManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if(m_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);

        m_UIManager = FindObjectOfType<UIManager>();

    }

    private void Start()
    {
        m_outOfCombatMusic.Play();
        m_inCombatMusic.Play();


        m_inCombatVolume = 0;
        m_outOfCombatVolume = 1;

        m_inCombatMusic.volume = m_inCombatVolume;
        m_outOfCombatMusic.volume = m_outOfCombatVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene a_scene, LoadSceneMode a_mode)
    {
        if (a_scene.name == m_UIManager.GetPlayScene())
        {
            m_playerInput = FindObjectOfType<PlayerInput>();
            m_musicTransition = m_playerInput.InCombat;
        }
    }

    private void Update()
    {
        PlayMenuMusic();
        FinishedCombat();
        TransitionBetween(m_musicTransition);
    }

    void TransitionBetween(bool a_toCombat)
    {
        if (!m_UIManager.GetInMenu())
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
    }

    void PlayMenuMusic()
    {
        if (m_UIManager.GetInMenu() == true)
        {
            m_mainMenuMusic.Play();
            m_outOfCombatMusic.Stop();
        }
        else if (m_UIManager.GetInMenu() == false)
        {
            m_mainMenuMusic.Stop();
            m_outOfCombatMusic.Play();
        }
    }

    void FinishedCombat()
    {
        if (!m_UIManager.GetInMenu() && m_playerInput != null)
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
}
