using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 24/07/2018
//Last edited 28/07/2018

public class MoveCamera : MonoBehaviour {

    [SerializeField] private GameObject m_player;
    [SerializeField] private float m_xModifyer;
    [SerializeField] private float m_zModifyer;
    void Update ()
    {
        Vector3 position = new Vector3(m_player.transform.position.x + m_xModifyer, transform.position.y, m_player.transform.position.z + m_zModifyer);
        transform.position = position;
    }
}
