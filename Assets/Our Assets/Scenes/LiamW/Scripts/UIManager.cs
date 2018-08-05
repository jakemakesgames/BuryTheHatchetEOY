using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    //-------Public Variables------//
    #region Public Variables

    public GameObject m_pauseScreen;
    public GameObject m_pauseMenu;
    public GameObject m_optionsMenu;
    public GameObject m_bountyScreen;
    public GameObject m_bountyBoard;
    public GameObject m_bountyBoardScreen;
    public GameObject m_bountyInteractionScreen;

    public Slider m_uiMainSlider;
    public Slider m_uiMusicSlider;
    public Slider m_uiSFXSlider;

    //*! Singleton attempt
    public static UIManager m_Instance;

    #endregion

    #region Private Variables

    bool m_hasBounty;
    bool m_isPaused;
    bool m_inBounty;
    bool m_inPausedMenu;
    bool m_soldOut;

    Interactable m_interactable;
    SoundManager m_soundManager;

    #endregion


    #region Getter and Setter

    public bool GetHasBounty()
    {
        return m_hasBounty;
    }
    public bool GetIsPaused()
    {
        return m_isPaused;
    }


    public void SetHasBounty(bool a_hasBounty)
    {
        m_isPaused = a_hasBounty;
    }
    public void SetIsPaused(bool a_isPaused)
    {
        m_isPaused = a_isPaused;
    }

    public float GetMainVolume()
    {
        return m_uiMainSlider.value;
    }

    #endregion

    #region Private Functions

    private void Awake()
    {
        m_Instance = this;
    }


    private void Start()
    {
        Time.timeScale = 1;
        m_pauseScreen.SetActive(false);
        m_pauseMenu.SetActive(true);
        m_bountyScreen.SetActive(false);
        m_bountyBoard.SetActive(false);
        m_bountyBoardScreen.SetActive(false);
        m_bountyInteractionScreen.SetActive(false);

        m_soundManager = SoundManager.m_instance;
    }

    #endregion

    #region Public Functions

    public void ChangeScenes(int changeScene)
    {
        //Loads our games Scene
        SceneManager.LoadScene(changeScene, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Update()
    {
        #region Pause Screen Controls
        if (Input.GetKeyDown("escape") && !m_isPaused)
        {
            if (!m_inBounty)
            {
                //Set any screen that is not the pause screen off
                m_inPausedMenu = true;
                m_isPaused = true;
                m_bountyInteractionScreen.SetActive(false);
                m_pauseScreen.SetActive(true);
                m_pauseMenu.SetActive(true);


                //Sets the game loop speed to 0
                Time.timeScale = 0;

                Debug.Log("Game is Paused");
            }

        }
        else if (Input.GetKeyDown("escape") && m_isPaused)
        {
            if (!m_inBounty)
            {
                //Checks if the Pause Menu is active
                if (m_pauseMenu.activeSelf)
                {
                    m_isPaused = false;
                    Time.timeScale = 1;

                    //Turn all other screens off
                    m_pauseScreen.SetActive(false);
                    m_bountyBoard.SetActive(false);
    
                    m_inPausedMenu = false;
                    Debug.Log("Game Unpaused");
                }
                else
                {
                    m_optionsMenu.SetActive(false);
                    m_bountyBoard.SetActive(false);
                    m_pauseMenu.SetActive(true);
                }  
            }
        }
        #endregion

        #region Bounty Screen Controls

        if (Input.GetKeyDown("b") && !m_isPaused && m_hasBounty)
        {
            if (!m_inPausedMenu)
            {
                m_inBounty = true;

                m_isPaused = true;
                m_pauseScreen.SetActive(false);
                m_bountyScreen.SetActive(true);
                Time.timeScale = 0;

                Debug.Log("Looking at Bounty");
            }
        }
        else if (Input.GetKeyDown("b") && m_isPaused)
        {
            if (!m_inPausedMenu)
            {
                m_inBounty = false;
                m_isPaused = false;
                m_bountyScreen.SetActive(false);
                m_bountyBoard.SetActive(false);

                Time.timeScale = 1;

                Debug.Log("Puts Bounty Away");
            }
        }
        else if (Input.GetKeyDown("escape") && m_isPaused)
        {
            if (!m_inPausedMenu)
            {
                m_inBounty = false;
                m_isPaused = false;
                m_bountyScreen.SetActive(false);

                Time.timeScale = 1;

                Debug.Log("Game is UnPaused");
            }
        }

        #endregion

        #region Sound Controls

        MainGameMusic();

        PlayerPrefs.GetFloat("m_mainVolumeValue", m_soundManager.GetMainVolume());

        #endregion
    }

    public void Unpause()
    {
        if (m_isPaused)
        {
            m_isPaused = false;
            m_inPausedMenu = false;
            m_pauseMenu.SetActive(false);
            m_bountyScreen.SetActive(false);

            //Sets the game loop speed to 0
            Time.timeScale = 1;

            Debug.Log("Game is UnPaused");
        }
    }

    public void AcceptBounty()
    {
        m_hasBounty = true;
    }

    public void Back()
    {
        if (m_isPaused)
        {
            
        }
    }

    public void Shop()
    {

    }

    public void MainGameMusic()
    {
        m_uiMainSlider.value = m_soundManager.m_mainSlider.value;

        m_soundManager.m_mainVolume.volume = m_uiMainSlider.value;

        PlayerPrefs.SetFloat("MainGameMusic", m_uiMainSlider.value);
    }

    #endregion
}