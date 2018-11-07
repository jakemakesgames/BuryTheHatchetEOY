﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Camera Requirements
// 1 - Create an Offset from the camera to the player
// 2 - Smooth movement, example camera drags slightly behind the player as they traverse through the game
// 3 - When the player dies the camera fades to black then when the player is reset it fades to white (UI Panel)
// 4 - Screen Shake, when the player gets hit there will be a screen shake value that is used that effects the rotation of the camera
// Refer to https://www.youtube.com/watch?v=tu-Qe66AvtY&list=PLpBhLW3iKfP5GoNLWqPTdEZ0nwRFOQkS6&index=9
// And https://www.youtube.com/watch?v=C7307qRmlMI&index=9&list=PLpBhLW3iKfP5GoNLWqPTdEZ0nwRFOQkS6

// Sebastian Lague

public class PlayerCamera : MonoBehaviour
{
    #region Public Variables
    [Header("Player")]
    [Tooltip("Player Character")]
    [SerializeField] Player m_player;
    [Tooltip("Camera's offset to the Player Character")]
    [SerializeField] Vector3 m_offset;

    [Header("Camera's following speed")]
    [Tooltip("The Smoothness in which our camera follows our player")]
    [Range(0, 100)] [SerializeField] float m_smoothSpeed;



    #endregion

    #region Private Variables

    private UIManager m_uiManager;

    private Scene m_scene;

    public static PlayerCamera m_instance;

    //Target the Camera is going to reach
    private Vector3 m_desiredPosition;

    //The smoothness in which the camera will reach the desired positions
    private Vector3 m_smoothedPosition;

    private bool m_foundPlayer = true;

    private void Awake()
    {

        m_uiManager = FindObjectOfType<UIManager>();

        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode a_mode)
    {
        if (scene.name == m_uiManager.GetPlayScene())
        {
            m_player = (Player)FindObjectOfType((typeof(Player)));
        }
    }

    #endregion

    // Update is called once per frame
    void Update ()
    {
        PlayerCheck();
    }

 
     public void PlayerCheck()
    {
 
        if (m_player != null)
        {
            CameraFollow();
        }
    }

    public void CameraFollow()
    {
        if (m_foundPlayer)
        {
            #region Smoothed Camera Follow
            //Sets the players position to be the desired position
            m_desiredPosition = m_player.transform.position + m_offset;
            //Lerps the camera between its initial position to the targets position by the smooth speed
            m_smoothedPosition = Vector3.Lerp(transform.position, m_desiredPosition, m_smoothSpeed / 100);

            //Makes the Camera follow the Player
            transform.position = m_smoothedPosition;
            #endregion
        }
    }
}
