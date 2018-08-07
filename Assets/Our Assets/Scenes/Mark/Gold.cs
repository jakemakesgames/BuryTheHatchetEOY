using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Gold : MonoBehaviour {

    [SerializeField] private int m_goldDropMin;
    [SerializeField] private int m_goldDropMax;

    private bool goldFlag = false;

    public void SetGoldFlag(bool a_goldFlag) { goldFlag = a_goldFlag; }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
