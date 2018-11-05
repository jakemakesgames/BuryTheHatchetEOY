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
    [Tooltip("Credits object")]
    [SerializeField] GameObject m_creditsMenu;
    [Tooltip("Pause Menu object")]
    [SerializeField] GameObject m_pauseMenu;
    
    [Header("Scene Names")]
    [Tooltip("Game Scene string")]
    [SerializeField] string m_playScene;
    [Tooltip("Menu Scene string")]
    [SerializeField] string m_menuScene;
    [Tooltip("Menu Scene string")]
    [SerializeField] string m_creditsScene;
    [Tooltip("Player character")]
    [SerializeField] Player m_player;
    [Tooltip("Players Health Bar")]
    [SerializeField] Image m_health;
    [Tooltip("Fade time from Main Menu to Game Scene")]
    [SerializeField] float m_fadeTime;
    [Tooltip("Fade time from Game Scene to Menu")]
    [SerializeField] float m_menuFadeTime;
    [Tooltip("Time it takes for the Credit scene to fade")]
    [SerializeField] float m_creditFadeTime;

    [SerializeField] Animator m_camerFadeAnim;

    private bool m_isPaused;
    private bool m_inMenu;
    private bool m_inCredit;
    private bool m_fade = true;
    private bool m_finishedMenuFade;
    private bool m_fading;
    private float m_maxHealth;
    private float m_currHealth;
    private int m_fadeLevel;
    private Scene m_currScene;
    private SoundManager m_soundManager;

    private WeaponController m_weaponController;

    public static UIManager m_instance;

    private void Awake()
    {
        Time.timeScale = 1;
        m_inMenu = true;

        //Makes sure there is only ever one instance of this GameObject
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        m_mainMenu.SetActive(true);
        m_optionsMenu.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_creditsMenu.SetActive(false);

        //CurrentEquippedWeaponImage();

        m_camerFadeAnim.gameObject.SetActive(false);

        m_currScene = SceneManager.GetActiveScene();

        m_soundManager = FindObjectOfType<SoundManager>();

    }

    public bool GetInMenu()
    {
        return m_inMenu;
    }

    public bool CreditScene { get { return m_inCredit; } }

    public string GetPlayScene()
    {
        return m_playScene;
    }

    public void Update()
    {
        if (m_inMenu == false)
        {
            if (Input.GetKeyDown("escape") && m_isPaused == false)
            {
                Pause();
            }
            else if (Input.GetKeyDown("escape") && m_isPaused == true)
            {
                Unpause();
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode a_mode)
    {
        if (scene.name == m_playScene)
        {
            m_player = (Player)FindObjectOfType((typeof(Player)));
        }
    }

    private IEnumerator WaitForFade(float a_WaitTime, string a_scene)
    {
        FadeToNextLevel();
        yield return new WaitForSeconds(a_WaitTime);
        m_camerFadeAnim.ResetTrigger("FadeOut");
        SceneManager.LoadScene(a_scene);

        if (m_inMenu)
        {
            m_mainMenu.SetActive(true);
        }

        if (m_inCredit)
        {
            m_mainMenu.SetActive(false);
            m_creditsMenu.SetActive(true);
        }
    }

    #region Public Functions

    [ContextMenu("Play Game")]
    public void PlayGame()
    {
        m_camerFadeAnim.gameObject.SetActive(true);
        FadeOutOfLevel();
        StartCoroutine(WaitForFade(m_fadeTime, m_playScene));

        m_mainMenu.SetActive(false);
        m_inMenu = false;

    }

    public void FadeOutOfLevel()
    {
        m_camerFadeAnim.SetTrigger("FadeOut");
    }
    
    public void FadeToNextLevel()
    {
        m_camerFadeAnim.SetTrigger("FadeIn");
    }

    [ContextMenu("Options")]
    public void Options()
    {
        m_optionsMenu.SetActive(true);
        m_mainMenu.SetActive(false);
        m_pauseMenu.SetActive(false);

    }

    [ContextMenu("Pause Game")]
    public void Pause()
    {
        m_isPaused = true;
        Time.timeScale = 0;
        m_pauseMenu.SetActive(true);
        m_optionsMenu.SetActive(false);
        m_mainMenu.SetActive(false);
    }

    [ContextMenu("Unpause Game")]
    public void Unpause()
    {
        Time.timeScale = 1;
        m_isPaused = false;
        m_pauseMenu.SetActive(false);
        m_optionsMenu.SetActive(false);
    }

    [ContextMenu("Return to Menu")]
    public void ReturnToMenu()
    {
        m_pauseMenu.SetActive(false);
        m_creditsMenu.SetActive(false);
        Time.timeScale = 1;
        FadeOutOfLevel();
        StartCoroutine(WaitForFade(m_menuFadeTime, m_menuScene));

        m_inMenu = true;
        m_inCredit = false;
        m_soundManager.m_exitCreditSound = true;

    }

    [ContextMenu("Back")]
    public void Back()
    {
        if (m_inMenu == true)
        {
            m_optionsMenu.SetActive(false);
            m_mainMenu.SetActive(true);
        }
        else if (m_inMenu == false)
        {
            m_pauseMenu.SetActive(true);
            m_optionsMenu.SetActive(false);
        }
    }

    [ContextMenu("Credits")]
    public void Credits()
    {
        m_camerFadeAnim.gameObject.SetActive(true);
        FadeOutOfLevel();
        StartCoroutine(WaitForFade(m_creditFadeTime, m_creditsScene));

        m_soundManager.m_creditSound = true;
        m_inCredit = true;
        m_soundManager.m_exitCreditSound = false;
    }

    [ContextMenu("Quit Game")]
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void DeathFade()
    {
        //Plays the Fade to Black animation
        m_camerFadeAnim.SetTrigger("DeathFadeOut");
    }

    public void RespawnFade()
    {
        //Plays the Fade from Black animation
        m_camerFadeAnim.SetTrigger("FadeIn");
    }

    #endregion
}

    #region OLD GUI CODE LIAM
/*

        //private void CurrentEquippedWeaponImage()
    //{
    //    if (m_player != null)
    //    {
    //        if (m_player.HeldWeaponLocation == 1)
    //        {
    //            //Turns the Revolver Bullets off
    //            m_revBulletImage.SetActive(false);
    //            //Put in Hatchet image
    //            m_hatchetImage.SetActive(true);
    //        }
    //
    //        if (m_player.HeldWeaponLocation == 2)
    //        {
    //            //Turns the Hatchet off
    //            m_hatchetImage.SetActive(false);
    //            //Put in Revolver image
    //            m_revBulletImage.SetActive(true);
    //        }
    //    }  
    //}

    //public void RestartLevel()
    //{
    //    Time.timeScale = 1;
    //    m_endLevel.SetActive(false);
    //    StartCoroutine(WaitForFade(m_fadeTime, m_playScene));
    //}

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

*/
    #endregion
