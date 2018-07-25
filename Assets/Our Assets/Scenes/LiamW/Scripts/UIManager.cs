using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    //-------Public Variables------//
    #region public variables

    public GameObject pauseMenu;
    public GameObject pausePanel;
    public GameObject optionsPanel;
    
    
    
    bool isPaused;
    bool m_soldOut;
    float m_soundEffect;
    float m_mainVolume;

    #endregion

    private void Start()
    {
        pauseMenu.SetActive(false);
    }

    #region public functions
    public void StartGame(int changeScene)
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
            pauseMenu.SetActive(true);

            //Sets the game loop speed to 0
            Time.timeScale = 0;

            Debug.Log("Game is Paused");

        }
        else if (Input.GetKeyDown("escape") && isPaused)
        {
            //Checks if the pause panel is active
            if (pausePanel.activeSelf)
            {
                isPaused = false;
                Time.timeScale = 1;
                pauseMenu.SetActive(false);
                Debug.Log("Game Unpaused");
            }
            else
            {
                optionsPanel.SetActive(false);
                pausePanel.SetActive(true);
            }
            
        }
    }

    public void Unpause()
    {
        if (isPaused)
        {
            isPaused = false;
            pauseMenu.SetActive(false);

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
