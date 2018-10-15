using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : MonoBehaviour
{
    [Header("Lerp rotation")]
    [Tooltip("Left view rotation")]
    [SerializeField] Transform m_leftView;
    [Tooltip("Right look rotation")]
    [SerializeField] Transform m_rightView;
    [Tooltip("Speed of the rotation")]
    [SerializeField] float m_rotationSpeed = 0.1f;

    Vector3 m_relativePosition;
    Quaternion m_rightRotation;
    Quaternion m_leftRotation;
    float m_timer = 0;
    bool m_isRotating = true;
    bool m_countingUp = true;

	// Use this for initialization
	void Start ()
    {
        m_rightRotation = Quaternion.LookRotation(m_leftView.position);
        m_leftRotation = Quaternion.LookRotation(m_rightView.position);
    }
	
	// Update is called once per frame
	void Update ()
    {
        SentryLerp();
	}

    void SentryLerp()
    {
        if (m_isRotating == true)
        {

            m_relativePosition = m_rightView.position - transform.position;

            if (m_countingUp)
            {
                m_timer += Time.deltaTime;
            }
            else
            {
                m_timer -= Time.deltaTime;
            }

            if (m_timer >= 1)
            {
                m_countingUp = false;
            }
            else if (m_timer <= 0)
            {
                m_countingUp = true;
            }

            transform.rotation = Quaternion.Slerp(m_leftRotation, m_rightRotation, m_timer);
        }

    }
}
