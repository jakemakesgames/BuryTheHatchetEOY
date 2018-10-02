using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 17/09/2018
//Last edited 19/09/2018

public class InteractableMinecart : MonoBehaviour {
    [SerializeField] private float m_playerMoveSpeedMultipler;
    
    [SerializeField] private string m_playerTag;
    private PlayerInput m_player;
    //when the player enters this trigger range set the players respawn point to this transform
    private void OnTriggerEnter(Collider other) {
        if (other.tag == m_playerTag) {
            m_player = other.GetComponent<PlayerInput>();
            if (m_player != null) {
                m_player.CurrentlyCanInteractWith = PlayerInput.InteractableObject.MINECART;
                m_player.InteractionObject = gameObject;
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
