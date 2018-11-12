using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroll : MonoBehaviour {

    public float speed;
    public GameObject backBtn;

    [Header("Back Button Timer")]
    private float timeCounter;
    public float timeCounterMax;

    public string sceneToLoad;

    [Header("End Credits Timer")]
    public float endTimer;

    private void Start()
    {
        backBtn.SetActive(false);
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
        SceneManager.LoadScene(sceneToLoad);
    }
}
