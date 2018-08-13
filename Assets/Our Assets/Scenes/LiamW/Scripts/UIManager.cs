using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//Get the bullet images disappearing when the player shoots
//Health go down when the player hits

public class UIManager : MonoBehaviour
{

    #region Public Variables

    public GameObject m_startMenu;
    public GameObject m_controlsMenu;
    public GameObject m_optionsMenu;
    public GameObject m_optionsPanel;


    public GameObject m_pauseMenu;
    public GameObject m_bountyPoster;
    public GameObject m_bountyBoard;
    public GameObject m_bountyInteraction;

    public GameObject m_playerHud;
    public Image m_health;

    public List<GameObject> m_revBullets;
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

    private float m_currHealth;
    private float m_maxHealth;

    private int m_numBullets;
    WeaponController m_weaponController;
    [SerializeField] Player m_player;
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
        m_bountyInteraction.SetActive(false);
        m_optionsMenu.SetActive(false);
        m_playerHud.SetActive(false);



        Time.timeScale = 0;
        m_soundManager = SoundManager.m_instance;
        m_weaponController = m_player.GetComponent<WeaponController>();
    }

    #endregion

    #region Public Functions

    public void Quit()
    {
        Application.Quit();
    }

    public void Update()
    {
        CurrentHealth();
        ClipDisplay();

        #region Main Menu Controls

        //In Main Menu
        if (m_inMainMenu)
        {
            //Set the Bounty Screens to false
            m_bountyBoard.SetActive(false);
            m_bountyInteraction.SetActive(false);
            m_playerHud.SetActive(false);

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

                        m_bountyInteraction.SetActive(false);
                        m_pauseMenu.SetActive(true);
                        m_playerHud.SetActive(false);

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

                    m_playerHud.SetActive(false);
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
            m_playerHud.SetActive(true);

            //Sets the game loop speed to 0
            Time.timeScale = 1;

            Debug.Log("Game is UnPaused");
        }
    }

    public void StartGame()
    {
        m_inMainMenu = false;
        m_startMenu.SetActive(false);
        m_playerHud.SetActive(true);
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

    public void ClipDisplay()
    {
        if (!m_inPausedMenu)
        {
            if (!m_inMainMenu)
            {
                if (m_weaponController.GetEquippedGun() != null)
                {
                    if (Input.GetMouseButton(0))
                    {
                        m_numBullets = m_weaponController.GetEquippedGun().GetCurrentClip();

                        for (int i = 0; i < m_revBullets.Count; i++)
                        {
                            m_revBullets[i].SetActive(false);
                        }

                        for (int i = 0; i < m_numBullets; i++)
                        {
                            m_revBullets[i].SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void CurrentHealth()
    {
        m_currHealth = m_player.GetHealth();
        m_maxHealth = m_player.GetMaxHealth();

        m_health.fillAmount = m_currHealth / m_maxHealth;
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
        m_bountyInteraction.SetActive(false);
        m_optionsMenu.SetActive(false);
        m_playerHud.SetActive(false);
    }

    #endregion
}