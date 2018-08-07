using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//I couldnt find where but Bounty Interaction and Bounty Board get set to true somewhere
//In the game loop need to check if the Main Menu is active, if it isnt open up the options menu
//Same with the Bounty Menu when you interact with the Bounty board and Bounty Screen when you obtain
//The Bounty

public class UIManager : MonoBehaviour
{
    //-------Public Variables------//
    #region Public Variables

    public GameObject m_startMenu;
    public GameObject m_controlsMenu;
    public GameObject m_optionsMenu;
    public GameObject m_optionsPanel;

    public GameObject m_pauseMenu;
    public GameObject m_bountyPoster;
    public GameObject m_bountyBoard;
    public GameObject m_bountyInteractionScreen;

    //*! Singleton attempt
    public static UIManager m_Instance;

    #endregion

    #region Private Variables

    bool m_hasBounty;
    bool m_isPaused;
    bool m_inBounty;
    bool m_inPausedMenu;
    bool m_soldOut;
    bool m_inMainMenu;
    

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

    #endregion

    #region Private Functions

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //Sets Only the Start Menu to active 
        m_inMainMenu = true;
        m_startMenu.SetActive(true);

        m_controlsMenu.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_bountyPoster.SetActive(false);
        m_bountyBoard.SetActive(false);
        m_bountyInteractionScreen.SetActive(false);
        m_optionsMenu.SetActive(false);

        Time.timeScale = 0;
        m_soundManager = SoundManager.m_instance;
    }

    #endregion

    #region Public Functions

    public void Quit()
    {
        Application.Quit();
    }

    public void Update()
    {

        #region Main Menu Controls

        //In Main Menu
        if (m_inMainMenu)
        {
            //Set the Bounty Screens to false
            m_bountyBoard.SetActive(false);
            m_bountyInteractionScreen.SetActive(false);

            //Takes you back to the Start Screen from the Options Menu
            if (Input.GetKeyDown("escape") && m_optionsMenu.activeSelf)
            {
                m_optionsMenu.SetActive(false);
                m_startMenu.SetActive(true);

                Debug.Log("In Options Menu while Main Menu is active");
            }

            //Takes you back to the Start Screen from the Controls Menu
            if (Input.GetKeyDown("escape") && m_controlsMenu.activeSelf)
            {
                m_controlsMenu.SetActive(false);
                m_startMenu.SetActive(true);

                Debug.Log("In Options Menu while Main Menu is active");
            }

        }

        #endregion

        #region Pause Screen Controls

        //If it is not in the Main Menu
        if (!m_inMainMenu)
        {
            //If the pause Menu is active
            if (m_pauseMenu)
            {

                m_optionsPanel.SetActive(false);

                //If the game is currently not paused
                if (Input.GetKeyDown("escape") && !m_isPaused)
                {
                    if (!m_inBounty)
                    {
                        //Set any screen that is not the pause screen off
                        m_inPausedMenu = true;
                        m_isPaused = true;

                        m_bountyInteractionScreen.SetActive(false);
                        m_pauseMenu.SetActive(true);


                        //Sets the game loop speed to 0
                        Time.timeScale = 0;

                        Debug.Log("Game is Paused");
                    }

                }
                else if (Input.GetKeyDown("escape") && m_isPaused)
                {
                    //If we are currently not in the Bounty Board
                    if (!m_inBounty)
                    {
                        //Checks if the Pause Menu is active
                        if (m_pauseMenu.activeSelf)
                        {
                            m_isPaused = false;
                            Time.timeScale = 1;

                            //Turn all other screens off
                            m_pauseMenu.SetActive(false);
                            m_bountyBoard.SetActive(false);
    
                            m_inPausedMenu = false;
                            Debug.Log("Game Unpaused");
                        }
                        else
                        {
                            //If the Pause Menu is not active turn it on
                            m_bountyBoard.SetActive(false);
                            m_optionsMenu.SetActive(false);
                            m_pauseMenu.SetActive(true);

                            Debug.Log("Game is paused");
                        }  
                    }
                }
            }
        }

        #endregion

        #region Bounty Screen Controls

        if (!m_inMainMenu)
        {
            //Brings up the Bounty Poster when you obtain it from the Bounty Board
            if (Input.GetKeyDown("b") && !m_isPaused && m_hasBounty)
            {
                if (!m_inPausedMenu)
                {
                    //Pauses the game and displays the Bounty Poster
                    m_inBounty = true;

                    m_isPaused = true;

                    m_pauseMenu.SetActive(false);
                    m_bountyPoster.SetActive(true);
                    Time.timeScale = 0;

                    Debug.Log("Looking at Bounty");
                }
            }

            //Closes the Bounty Poster
            else if (Input.GetKeyDown("b") && m_isPaused)
            {
                if (!m_inPausedMenu)
                {
                    //Resumes the game
                    m_inBounty = false;
                    m_isPaused = false;
                    m_bountyPoster.SetActive(false);
                    m_bountyBoard.SetActive(false);

                    Time.timeScale = 1;

                    Debug.Log("Puts Bounty Away");
                }
            }
            else if (Input.GetKeyDown("escape") && m_isPaused)
            {
                if (!m_inPausedMenu)
                {
                    //Resumes the game
                    m_inBounty = false;
                    m_isPaused = false;
                    m_bountyPoster.SetActive(false);

                    Time.timeScale = 1;

                    Debug.Log("Game is UnPaused");
                }
            }
        }

            #endregion

        #region Sound Controls

        if (m_soundManager != null)
        {
            m_soundManager.MainVolume();
        }

        #endregion
    }

    public void Unpause()
    {
        if (m_isPaused)
        {
            m_isPaused = false;
            m_inPausedMenu = false;
            m_inBounty = false;
            m_bountyPoster.SetActive(false);

            //Sets the game loop speed to 0
            Time.timeScale = 1;

            Debug.Log("Game is UnPaused");
        }
    }

    public void StartGame()
    {
        m_inMainMenu = false;
        m_startMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void AcceptBounty()
    {
        m_hasBounty = true;
    }

    public void Back()
    {
        if (!m_inMainMenu)
        {
            if (m_isPaused)
            {
                m_optionsMenu.SetActive(false);
                if(m_pauseMenu != null)
                {
                    m_pauseMenu.SetActive(true);
                }
            }
        }
        else
        {
            Time.timeScale = 0;

            m_startMenu.SetActive(true);
            m_optionsPanel.SetActive(true);
        }
    }

    public void Shop()
    {

    }

    public void StartMenu()
    {
        Time.timeScale = 0;

        m_inMainMenu = true;
        m_startMenu.SetActive(true);
        m_optionsPanel.SetActive(true);

        m_controlsMenu.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_bountyPoster.SetActive(false);
        m_bountyBoard.SetActive(false);
        m_bountyInteractionScreen.SetActive(false);
        m_optionsMenu.SetActive(false);
    }

    #endregion
}