using UnityEngine;

public class Interactable : MonoBehaviour
{

    UIManager m_uiManager;
    bool m_isInteracting;


    public static Interactable m_instance;

    public bool GetIsInteracting()
    {
        return m_isInteracting;
    }
    public void SetIsInteracting(bool a_isInteracting)
    {
        m_isInteracting = a_isInteracting;
    }

    private void Awake()
    {
        m_instance = this;
    }

    private void Start()
    {
        m_uiManager = UIManager.m_Instance;
    }

    private void Update()
    {
        
    }


    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            m_uiManager.m_bountyBoardScreen.SetActive(true);
            m_uiManager.m_bountyInteractionScreen.SetActive(true);

            if (!m_uiManager.GetHasBounty())
            {
                if (Input.GetKeyDown("e") && !m_uiManager.GetIsPaused())
                {
                    m_isInteracting = true;
                    m_uiManager.SetIsPaused(true);

                    m_uiManager.m_bountyInteractionScreen.SetActive(false);
                    m_uiManager.m_bountyBoard.SetActive(true);
                    Time.timeScale = 0;


                    Debug.Log("Player is looking at Bounty");
                }
                else if (Input.GetKeyDown("e") && m_uiManager.GetIsPaused())
                {
                    m_isInteracting = false;
                    m_uiManager.SetIsPaused(false);
                    m_uiManager.m_bountyBoard.SetActive(false);


                    m_uiManager.Unpause();

                    Debug.Log("Player stopped looking at Bounty");
                }
            }
            //Debug.Log("Interacting");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_uiManager.m_bountyInteractionScreen.SetActive(false);
            Debug.Log("Leaving interactable area");
        }
    }
}
