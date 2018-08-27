using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour, IDamagable
{
    #region Serialized Variables

    //Public Game Objects
    [SerializeField] GameObject m_boss;
    [SerializeField] GameObject m_mineCart;
    [SerializeField] GameObject m_leftSide;
    [SerializeField] GameObject m_rightSide;

    //Public Variables
    [SerializeField] float m_walkSpeed;
    [SerializeField] float m_cartSpeed;
    [SerializeField] int m_cooldownTime;
    [SerializeField] int m_overheatTime;

    #endregion

    #region Private Variables

    private bool m_barrelsDestroyed;

    #endregion


    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    #region Public Functions

    public void Walking()
    {

    }

    public void Minecart()
    {
        


    }

    public void MachineGun()
    {

    }

    public void TakeHit(int a_damage, RaycastHit a_hit)
    {
        
    }

    public void TakeDamage(int a_damage)
    {
        
    }

    public void TakeImpact(int a_damage, RaycastHit a_hit, Projectile a_projectile)
    {
        
    }

    #endregion
}
