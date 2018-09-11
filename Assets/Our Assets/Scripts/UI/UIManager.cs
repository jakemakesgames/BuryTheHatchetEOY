using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//Get the bullet images disappearing when the player shoots
//Health go down when the player hits

public class UIManager : MonoBehaviour
{
    [Header("Menu GameObjects")]
    [Tooltip("Main Menu object")]
    [SerializeField] GameObject m_mainMenu;
    [Tooltip("Options Menu object")]
    [SerializeField] GameObject m_optionsMenu;
    [Tooltip("Pause Menu object")]
    [SerializeField] GameObject m_pauseMenu;

    [Header("Scene Names")]
    [Tooltip("Game Scene string")]
    [SerializeField] string m_playScene;
    [Tooltip("Menu Scene string")]
    [SerializeField] string m_menuScene;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        m_mainMenu.SetActive(true);
        m_optionsMenu.SetActive(false);

        if (m_pauseMenu != null)
        {
            m_pauseMenu.SetActive(false);
        }
    }


    #region Public Functions

    public void PlayGame()
    {
        SceneManager.LoadScene(m_playScene);
    }

    public void Options()
    {
        m_optionsMenu.SetActive(true);
        m_mainMenu.SetActive(false);

        if (m_pauseMenu != null)
        {
            m_pauseMenu.SetActive(false);
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        m_pauseMenu.SetActive(true);
        m_optionsMenu.SetActive(false);
        m_mainMenu.SetActive(false);
    }

    public void Unpause()
    {
        Time.timeScale = 1;
        m_pauseMenu.SetActive(false);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(m_menuScene);
    }

    public void Back()
    {
        m_optionsMenu.SetActive(false);
        m_mainMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #endregion
}
/*
    #region LIAM
    #region Public Variables

    [SerializeField] GameObject m_startMenu;
    [SerializeField] GameObject m_controlsMenu;
    [SerializeField] GameObject m_optionsMenu;
    [SerializeField] GameObject m_optionsPanel;
    [SerializeField] GameObject m_pauseMenu;
    [SerializeField] GameObject m_bountyPoster;
    [SerializeField] GameObject m_playerHud;
    [SerializeField] Player m_player;
    [SerializeField] Image m_health;

    [SerializeField] List<GameObject> m_revBullets;
    [SerializeField] SoundManager m_soundManager;

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
    bool m_inCombat;

    private float m_currHealth;
    private float m_maxHealth;

    private int m_numBullets;
    private WeaponController m_weaponController;
    private Interactable m_interactable;

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

    #region Public Functions

    public void Quit()
    {
        Application.Quit();
    }

    public void Update()
    {
        CurrentHealth();

        if (m_inCombat)
        {
            ClipDisplay();
        }

        #region Main Menu Controls

        //In Main Menu
        if (m_inMainMenu)
        {
            //Set the Bounty Screens to false
            m_playerHud.SetActive(false);
            m_inCombat = false;

            //Takes you back to the Start Screen from the Options Menu
            if (Input.GetKeyDown("escape") && m_optionsMenu.activeSelf)
            {
                m_soundManager.BackSound();

                m_optionsMenu.SetActive(false);
                m_startMenu.SetActive(true);

                Debug.Log("In Options Menu while Main Menu is active");
            }

            //Takes you back to the Start Screen from the Controls Menu
            if (Input.GetKeyDown("escape") && m_controlsMenu.activeSelf)
            {
                m_soundManager.BackSound();

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
                        //m_soundManager.BackSound();

                        //Set any screen that is not the pause screen off
                        m_inPausedMenu = true;
                        m_isPaused = true;

                        m_playerHud.SetActive(false);
                        m_pauseMenu.SetActive(true);
                        m_inCombat = false;

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
                            m_soundManager.BackSound();

                            m_isPaused = false;
                            Time.timeScale = 1;

                            //Player HUD and Firing
                            m_playerHud.SetActive(true);
                            m_inCombat = true;

                            //Turn all other screens off
                            m_pauseMenu.SetActive(false);
    
                            m_inPausedMenu = false;
                            Debug.Log("Game Unpaused");
                        }
                        else
                        {
                            m_soundManager.BackSound();

                            //If the Pause Menu is not active turn it on
                            m_optionsMenu.SetActive(false);
                            m_pauseMenu.SetActive(true);

                            Debug.Log("Game is paused");
                        }  
                    }
                }
            }
        }

        #endregion
    }

    public void Unpause()
    {
        //Unpauses the game and turns off the Pause Screen
        if (m_isPaused)
        {
            m_isPaused = false;
            m_inPausedMenu = false;
            m_inBounty = false;
            m_bountyPoster.SetActive(false);
            m_playerHud.SetActive(true);
            m_inCombat = true;

            //Sets the game loop speed to 0
            Time.timeScale = 1;

            Debug.Log("Game is UnPaused");
        }
    }

    public void StartGame()
    {
        m_inMainMenu = false;
        m_startMenu.SetActive(false);

        //Player Hud and Firing True
        m_playerHud.SetActive(true);
        m_inCombat = true;
        Time.timeScale = 1;
    }

    public void AcceptBounty()
    {
        m_hasBounty = true;
    }

    public void Back()
    {
        //For the Back Button
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
        if (!m_inBounty && !m_inMainMenu && !m_inPausedMenu)
        {
            if (m_weaponController != null)
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
        //Grabs the health of the player and fills the bottle based on the players current health
        m_currHealth = m_player.GetHealth();
        m_maxHealth = m_player.GetMaxHealth();

        m_health.fillAmount = m_currHealth / m_maxHealth;
    }

    public void StartMenu()
    {
        //Pauses the game and sets the main menu screen to true
        Time.timeScale = 0;

        m_isPaused = false;
        m_inMainMenu = true;
        m_startMenu.SetActive(true);
        m_optionsPanel.SetActive(true);

        m_controlsMenu.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_bountyPoster.SetActive(false);
        m_optionsMenu.SetActive(false);
        m_playerHud.SetActive(false);
        m_inCombat = false;
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
        m_inMainMenu = false;
        m_startMenu.SetActive(false);

        m_controlsMenu.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_bountyPoster.SetActive(false);
        m_optionsMenu.SetActive(false);
        m_playerHud.SetActive(true);
        m_inCombat = false;



        Time.timeScale = 1;
        m_weaponController = m_player.GetComponent<WeaponController>();
    }

    #endregion

    #endregion
*/
