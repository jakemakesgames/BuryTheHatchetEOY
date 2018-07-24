using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    //-------Public Variables------//
    [SerializeField]
    private GameObject m_currentUI;
    public List<GameObject> m_uiPrefabs;



    //-------Public Variables------//
    bool isPaused;
    bool m_soldOut;
    float m_soundEffect;
    float m_mainVolume;



    void UpdateMenu()
    {

    }

    public void StartGame(int changeScene)
    {
        //Loads our games Scene
        SceneManager.LoadScene(changeScene);
    }

    public void Options()
    {

    }

    public void Controls()
    {

    }

    public void Quit()
    {

    }

    public void PauseGame()
    {

    }

    public void Back()
    {

    }

    public void AcceptBounty()
    {

    }

    public void SoundEffectControl()
    {

    }

    public void MainVolumeControl()
    {

    }
}
