using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save : MonoBehaviour
{

    public static Save m_gameCanvas;

	// Use this for initialization
	void Start ()
    {
        if (m_gameCanvas == null)
        {
            m_gameCanvas = this;
        }
        if (m_gameCanvas != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
