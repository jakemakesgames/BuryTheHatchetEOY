using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    #region Public Variables

    [Header("Sliders")]
    [SerializeField] Slider m_mainVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;
    [SerializeField] Slider m_SFXVolumeSlider;

    [Header("AudioSources")]
    [Tooltip("OutOfCombat Music")]
    [SerializeField] AudioSource m_outOfCombatMusic;
    [Tooltip("Combat Music")]
    [SerializeField] AudioSource m_inCombatMusic;
    [Tooltip("Main Menu Music")]
    [SerializeField] AudioSource m_mainMenuMusic;
    [Tooltip("Credits Music")]
    [SerializeField] AudioSource m_creditMusic;

    [Header("Volume Fade Controls")]
    [Tooltip("InCombatFade in speed")]
    [SerializeField] float m_fadeInControl;
    [Tooltip("InCombat Fade out speed")]
    [SerializeField] float m_fadeOutControl;

    [Header("Music Transition for In and Out of Combat")]
    [Tooltip("Timer for Music Transition to occur")]
    [SerializeField] float m_combatTime;
    [Tooltip("Delay time for OutOfCombatMusic to start playing when scene loads")]
    [SerializeField] float m_playOutOfCombatMusicTimer;

    #endregion

    #region Private Variables

    private bool m_musicTransition;
    private bool m_combatStatePrevFrame = false;
    private bool m_countingDown;
    private bool m_menuSound = true;
    public bool m_creditSound = true;
    public bool m_exitCreditSound;

    private float m_inCombatVolume;
    private float m_outOfCombatVolume;
    private float m_combatTimer;

    private PlayerInput m_playerInput;
    private UIManager m_UIManager;
    private static SoundManager m_instance;

    #endregion

    #region Private Functions

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
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
        m_creditMusic.Stop();


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
        PlayCreditMusic();
        VolumeController();
        TransitionBetween(m_musicTransition);
    }

    #region Combat and Passive music Transition

    private void PlayMenuMusic()
    {
        if (m_UIManager.GetInMenu() == true && m_menuSound == true)
        {
            m_mainMenuMusic.Play();
            m_outOfCombatMusic.Stop();
            m_inCombatMusic.Stop();
            m_menuSound = false;
        }
        else if (m_UIManager.GetInMenu() == false && m_menuSound == false)
        {
            m_mainMenuMusic.Stop();

            if (Time.time >= m_playOutOfCombatMusicTimer)
            {
                m_menuSound = true;
                m_outOfCombatMusic.Play();
                m_inCombatMusic.Play();
            }
        }
    }

    private void TransitionBetween(bool a_toCombat)
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
            m_inCombatMusic.volume = m_inCombatVolume * m_mainVolumeSlider.value * m_musicVolumeSlider.value;
            m_outOfCombatMusic.volume = m_outOfCombatVolume * m_mainVolumeSlider.value * m_musicVolumeSlider.value;
        }
    }

    private void FinishedCombat()
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
            else if (m_playerInput.InCombat)
            {
                m_musicTransition = true;
                m_countingDown = false;
            }

            m_combatStatePrevFrame = m_playerInput.InCombat;
        }
    }

    #endregion

    #endregion

    #region Public Functions

    public float MasterVolume { get { return m_mainVolumeSlider.value; } }

    public float MusicVolume { get { return m_musicVolumeSlider.value; } }

    public float SFXVolume { get { return m_SFXVolumeSlider.value; } }

    public void VolumeController()
    {
        m_mainMenuMusic.volume = m_mainVolumeSlider.value * m_musicVolumeSlider.value;
        m_creditMusic.volume = m_mainVolumeSlider.value * m_musicVolumeSlider.value;
    }

    public void PlayCreditMusic()
    {
        if (m_UIManager.CreditScene == true && m_creditSound == true)
        {
            m_creditMusic.Play();
            m_mainMenuMusic.Stop();
            m_creditSound = false;
        }
        else if (m_UIManager.CreditScene == false && m_creditSound == false)
        {
            m_creditMusic.Stop();
            if (m_UIManager.GetInMenu() && m_exitCreditSound == true)
            {
                m_mainMenuMusic.Play();
                m_exitCreditSound = false;
            }
        }
    }

    #endregion
}