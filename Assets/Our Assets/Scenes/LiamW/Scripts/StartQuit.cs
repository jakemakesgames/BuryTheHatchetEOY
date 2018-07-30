using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartQuit : MonoBehaviour
{
    public GameObject m_startMenu;
    public GameObject m_optionMenu;
    public GameObject m_controleMenu;


    public void Update()
    {
        if (Input.GetKeyDown("escape") && m_optionMenu)
        {
            m_optionMenu.SetActive(false);
            m_startMenu.SetActive(true);
        }

        if (Input.GetKeyDown("escape") && m_controleMenu)
        {
            m_controleMenu.SetActive(false);
            m_startMenu.SetActive(true);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame(int changeScene)
    {
        //Loads our games Scene
        SceneManager.LoadScene(changeScene);
    }
}
