using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 04/09/2018
//Last edited 04/09/2018

public class PlayerAnimationEvents : MonoBehaviour {
    [Tooltip("The player, needed to grab functions for")]
    [SerializeField] private PlayerInput m_player;

    public void EndSwing() {
        m_player.EndSwing();
    }

    public void EndRoll(){
        m_player.EndRoll();
    }
}
