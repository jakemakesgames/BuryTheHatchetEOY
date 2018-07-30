using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    //-------Public Variables------//
    #region public variables

    public GameObject m_pauseScreen;
    public GameObject m_pauseMenu;
    public GameObject m_optionsMenu;
    public GameObject m_bountyScreen;

    #endregion

    #region Private Variables

    bool m_hasBounty;
    bool m_isPaused;
    bool m_inBounty;
    bool m_inPausedMenu;
    bool m_soldOut;
    float m_soundEffect;
    float m_mainVolume;

    #endregion

    private void Start()
    {
        Time.timeScale = 1;
        m_pauseScreen.SetActive(false);
        m_pauseMenu.SetActive(true);
        m_bountyScreen.SetActive(false);
    }

    #region public functions
    public void ChangeScenes(int changeScene)
    {
        //Loads our games Scene
        SceneManager.LoadScene(changeScene);
    }

    void OnTriggerEnter(Collider BountyBoard)
    {

        if (Input.GetKeyDown("e"))
        {

            m_hasBounty = true;
        }
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
                m_inPausedMenu = true;
                m_isPaused = true;
                m_pauseScreen.SetActive(true);

                //Sets the game loop speed to 0
                Time.timeScale = 0;

                Debug.Log("Game is Paused");
            }

        }
        else if (Input.GetKeyDown("escape") && m_isPaused)
        {
            if (!m_inBounty)
            {
                //Checks if the pause panel is active
                if (m_pauseMenu.activeSelf)
                {
                    m_isPaused = false;
                    Time.timeScale = 1;
                    m_pauseScreen.SetActive(false);
                    m_inPausedMenu = false;
                    Debug.Log("Game Unpaused");
                }
                else
                {
                    m_optionsMenu.SetActive(false);
                    m_pauseMenu.SetActive(true);
                }  
            }
        }
        #endregion

        #region Bounty Screen Controls
        if (Input.GetKeyDown("b") && !m_isPaused)
        {
            if (!m_inPausedMenu)
            {
                m_inBounty = true;

                m_isPaused = true;
                m_pauseScreen.SetActive(false);
                m_bountyScreen.SetActive(true);
                Time.timeScale = 0;

                Debug.Log("Game is Paused");
            }
        }
        else if (Input.GetKeyDown("b") && m_isPaused)
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
    }

    public void Unpause()
    {
        if (m_isPaused)
        {
            m_isPaused = false;
            m_pauseScreen.SetActive(false);
            m_bountyScreen.SetActive(false);

            //Sets the game loop speed to 0
            Time.timeScale = 1;

            Debug.Log("Game is UnPaused");
        }
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

    public void SoundEffectControl()
    {

    }

    public void MainVolumeControl()
    {

    }
    #endregion
}