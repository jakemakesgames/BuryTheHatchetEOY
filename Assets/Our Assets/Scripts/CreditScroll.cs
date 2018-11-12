using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroll : MonoBehaviour {

    public float speed;
    public float m_toMenuFade;
    public string m_menuScene;
    public GameObject backBtn;

    private UIManager m_uiManager;

    [Header("Back Button Timer")]
    private float timeCounter;
    public float timeCounterMax;

    public string sceneToLoad;

    [Header("End Credits Timer")]
    public float endTimer;

    private void Start()
    {
        backBtn.SetActive(false);

        m_uiManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;

        transform.position += transform.up * speed * Time.deltaTime;

        if (timeCounter >= timeCounterMax) {
            backBtn.SetActive(true);
        }

        if (timeCounter >= endTimer) {
            BackToMenu();
        }
    }
    public void BackToMenu()
    {
        m_uiManager.FadeOutOfLevel();
        m_uiManager.m_inMenu = true;
        StartCoroutine(m_uiManager.WaitForFade(m_toMenuFade, m_uiManager.GetMenuScene()));
        //SceneManager.LoadScene(sceneToLoad);
    }
}
