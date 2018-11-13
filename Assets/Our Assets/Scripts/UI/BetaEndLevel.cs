using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BetaEndLevel : MonoBehaviour {

    public GameObject GameUI;

    //public string levelToReload;
    //public string menuToLoad;

    private UIManager m_uiManager;

    private void Start()
    {
        Time.timeScale = 1;
        GameUI.SetActive(true);
        m_uiManager = FindObjectOfType<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            Time.timeScale = 0;
            GameUI.SetActive(false);
            m_uiManager.EndLevelMenu.SetActive(true);
        }
    }

    public void ReloadScene() {
        //SceneManager.LoadScene(levelToReload);
    }

    public void QuitToMenu() {
        //SceneManager.LoadScene(menuToLoad);
    }
}
