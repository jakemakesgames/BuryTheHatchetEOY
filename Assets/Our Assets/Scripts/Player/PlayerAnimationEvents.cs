using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Michael Corben
//Created 04/09/2018
//Last edited 04/09/2018

public class PlayerAnimationEvents : MonoBehaviour {
    [Tooltip("The player, needed to grab functions for")]
    [SerializeField] private PlayerInput m_player;

    public void SlowingRoll() {
        if(m_player != null)
            m_player.SlowingRoll();
    }

    public void EndSwing() {
        if (m_player != null)
            m_player.EndSwing();
    }

    public void EndRoll(){
        if (m_player != null)
            m_player.EndRoll();
    }
}
