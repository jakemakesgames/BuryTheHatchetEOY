using UnityEngine;

//This script only holds the interaction for the Old Bounty Board

public class Interactable : MonoBehaviour
{
    #region Variables

    UIManager m_uiManager;
    
    bool m_isInteracting;
    public static Interactable m_instance;

    #endregion

    #region Setters
    public bool GetIsInteracting()
    {
        return m_isInteracting;
    }

    public void SetIsInteracting(bool a_isInteracting)
    {
        m_isInteracting = a_isInteracting;
    }
    #endregion

    #region Private functions
    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        //m_uiManager = UIManager.m_Instance;
    }

    private void Update()
    {
        
    }
    #endregion
}