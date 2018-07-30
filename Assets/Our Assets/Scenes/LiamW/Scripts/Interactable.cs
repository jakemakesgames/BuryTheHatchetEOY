using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float m_radius;
    public GameObject m_bountyTextScreen;

    public Transform m_player;
    float m_distance;

    private void Start()
    {
        m_bountyTextScreen.SetActive(false);
    }

    private void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }

    public void OnTriggerEnter(Collider other)
    {
        //Comnplete Tomorrow, get E working
        //float m_distance = (transform.position - transform.position)

        //if (m_distance <= m_radius)
        //{
        //    m_bountyTextScreen.SetActive(true);
        //    Debug.Log("Interacting");
        //}
        //else if (m_player.transform.r > m_radius)
        //{
        //    m_bountyTextScreen.SetActive(false);
        //}
    }
}
