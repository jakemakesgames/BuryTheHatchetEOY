using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 17/09/2018
//Last edited 02/10/2018

public class RespawnPoint : MonoBehaviour {

    [SerializeField] private Transform m_respawnPoint;

    [SerializeField] private string m_playerTag;
    private bool m_isCurrentRespawnPoint;
    private PlayerInput m_player;
    //when the player enters this trigger range set the players respawn point to this transform
    private void OnTriggerEnter(Collider other) {
        if (other.tag == m_playerTag) {
            m_player = other.GetComponent<PlayerInput>();
            if (m_player != null) {
                m_player.CurrentlyCanInteractWith = PlayerInput.InteractableObject.CAMPFIRE;
                m_player.InteractionObject = gameObject;
                m_player.Player.RespawnPoint = m_respawnPoint.position;
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == m_playerTag) {
            if (m_player != null)
                m_player.CurrentlyCanInteractWith = PlayerInput.InteractableObject.NONE;
        }
    }
}
