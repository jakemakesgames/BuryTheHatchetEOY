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

    Vector3 m_rightRelativePosition;
    Quaternion m_rightRotation;
    Vector3 m_leftRelativePosition;
    Quaternion m_leftRotation;
    float m_timer = 0;
    bool m_isRotating = true;
    bool m_countingUp = true;

	// Use this for initialization
	void Start ()
    {
        Quaternion targetPos = m_isRotating ? transform.rotation : m_rightView.rotation;
        Quaternion fromPos = m_isRotating ? transform.rotation : m_leftView.rotation;
    }
	
	// Update is called once per frame
	void Update ()
    {
        SentryLerp();
	}

    void SentryLerp()
    {
        if (m_countingUp)
        {
            m_timer += Time.deltaTime;
        }
        else
        {
            m_timer -= Time.deltaTime;
        }

        m_rightRelativePosition = m_rightView.position - transform.position;
        m_rightRotation = Quaternion.LookRotation(m_rightRelativePosition);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_rightRotation, m_timer);



        m_leftRelativePosition = m_leftView.position - transform.position;
        m_leftRotation = Quaternion.LookRotation(m_leftRelativePosition);
        transform.rotation = Quaternion.Lerp(m_leftRotation, m_leftRotation, m_timer);

    }
}
