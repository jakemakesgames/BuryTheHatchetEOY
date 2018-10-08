using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 17/09/2018
//Last edited 08/10/2018

public class RespawnPoint : MonoBehaviour {

    [SerializeField] private Transform m_respawnPoint;

    [SerializeField] private string m_playerTag;
    private bool m_isCurrentRespawnPoint;
    private PlayerInput m_player;

    //When the player enters this trigger range 
    //set the players respawn point to the set respawn point
    //Also set the currently interactable object to this
    private void OnTriggerEnter(Collider other) {
        if (other.tag == m_playerTag) {
            m_player = other.GetComponent<PlayerInput>();
            if (m_player != null) {
                m_player.CurrentlyCanInteractWith = PlayerInput.InteractableObject.RESPAWNPOINT;
                m_player.InteractionObject = gameObject;
                m_player.Player.RespawnPoint = m_respawnPoint.position;
            }
        }
    }

    //Set the players currently interactable object to nothing
    private void OnTriggerExit(Collider other) {
        if (other.tag == m_playerTag) {
            if (m_player != null)
                m_player.CurrentlyCanInteractWith = PlayerInput.InteractableObject.NONE;
        }
    }
}
