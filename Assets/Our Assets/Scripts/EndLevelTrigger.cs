using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevelTrigger : MonoBehaviour
{
    public UIManager m_uiManager;
    //public GameObject endLevel;
    public string restartLevel;
    public string menu;

    private void Start()
    {
        m_uiManager = FindObjectOfType<UIManager>();
        //endLevel.SetActive(false);
    }

    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        //endLevel.SetActive(true);
    //        Time.timeScale = 0;
    //        m_uiManager.m_endLevel.SetActive(true);
    //    }
    //} 

    public void Restart()
    {
        SceneManager.LoadScene(restartLevel);
    }

    public void LoadMenu()
    {
        //SceneManager.LoadScene(menu);
        m_uiManager.ReturnToMenu();
    }
}