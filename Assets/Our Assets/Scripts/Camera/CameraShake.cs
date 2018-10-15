using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Animator m_camAnim;

    private int m_randomShake;

    public void KickbackShake()
    {
        int m_numShakes = Random.Range(0, 3);

        if (m_numShakes == 0)
        {
            m_camAnim.SetTrigger("Shake");
        }
        else if(m_numShakes == 1)
        {
            m_camAnim.SetTrigger("Shake2");
        }
        else if(m_numShakes == 1)
        {
            m_camAnim.SetTrigger("Shake3");
        }
    }

    public void PlayerHitShake()
    {
        int m_moreShakes = Random.Range(0, 3);

        if (m_moreShakes == 0)
        {
            m_camAnim.SetTrigger("Shake4");
        }
        else if (m_moreShakes == 1)
        {
            m_camAnim.SetTrigger("Shake5");
        }
        else if (m_moreShakes == 2)
        {
            m_camAnim.SetTrigger("Shake6");
        }
    }
}
