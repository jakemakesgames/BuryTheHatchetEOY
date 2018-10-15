using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysFaceCamera : MonoBehaviour {

	private Transform myCamera;

	// Use this for initialization
	void Start () {
		myCamera = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (myCamera);
	}
}
