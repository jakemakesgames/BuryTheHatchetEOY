using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePickup : MonoBehaviour {
    enum PickUpType {
        Health = 0,
        Ammo = 1
    }
    [Tooltip("The type of pick up this will be")]
    [SerializeField] private PickUpType m_typeOfPickUp;
    [Tooltip("The amount of the pickup")]
    [SerializeField] private int m_value;
    
    [SerializeField] private string m_playerTag;
    private PlayerInput m_player;
    //when the player enters this trigger range set the players respawn point to this transform
    private void OnTriggerEnter(Collider other) {
        if (other.tag == m_playerTag) {
            m_player = other.GetComponent<PlayerInput>();
            if (m_player != null) {
                m_player.CurrentlyCanInteractWith = PlayerInput.InteractableObject.PICKUP;
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
