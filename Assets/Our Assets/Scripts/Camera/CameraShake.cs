using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Animator m_camAnim;
    [SerializeField] int m_numShakes;

    private int m_randomShake;

    public void ShakeCamera()
    {
        m_randomShake = Random.Range(0, m_numShakes);

        m_camAnim.SetInteger("ShakeNum", m_randomShake);
        m_camAnim.SetTrigger("Shake");
    }
}
