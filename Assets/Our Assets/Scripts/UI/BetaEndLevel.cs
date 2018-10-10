using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BetaEndLevel : MonoBehaviour {

    public GameObject GameUI;
    public GameObject GameOverUI;

    public string levelToReload;
    public string menuToLoad;

    private void Start()
    {
        Time.timeScale = 1;
        GameUI.SetActive(true);
        GameOverUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            Time.timeScale = 0;
            GameUI.SetActive(false);
            GameOverUI.SetActive(true);
        }
    }

    public void ReloadScene() {
        SceneManager.LoadScene(levelToReload);
    }

    public void QuitToMenu() {
        SceneManager.LoadScene(menuToLoad);
    }
}
