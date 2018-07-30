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
    
    
    
    bool isPaused;
    bool m_soldOut;
    float m_soundEffect;
    float m_mainVolume;

    #endregion

    private void Start()
    {
        Time.timeScale = 1;
        m_pauseScreen.SetActive(false);
        m_pauseMenu.SetActive(true);
    }

    #region public functions
    public void ChangeScenes(int changeScene)
    {
        //Loads our games Scene
        SceneManager.LoadScene(changeScene);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Update()
    {

        if (Input.GetKeyDown("escape") && !isPaused)
        {
            isPaused = true;
            m_pauseScreen.SetActive(true);

            //Sets the game loop speed to 0
            Time.timeScale = 0;

            Debug.Log("Game is Paused");

        }
        else if (Input.GetKeyDown("escape") && isPaused)
        {
            //Checks if the pause panel is active
            if (m_pauseMenu.activeSelf)
            {
                isPaused = false;
                Time.timeScale = 1;
                m_pauseScreen.SetActive(false);
                Debug.Log("Game Unpaused");
            }
            else
            {
                m_optionsMenu.SetActive(false);
                m_pauseMenu.SetActive(true);
            }  
        }
    }

    public void Unpause()
    {
        if (isPaused)
        {
            isPaused = false;
            m_pauseScreen.SetActive(false);

            //Sets the game loop speed to 0
            Time.timeScale = 1;

            Debug.Log("Game is UnPaused");
        }
    }

    public void Back()
    {
        if (isPaused)
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