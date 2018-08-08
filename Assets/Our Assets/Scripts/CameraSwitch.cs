using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour {

	public GameObject topCamera;
	public GameObject standardCamera;

	// Use this for initialization
	void Start () {
		topCamera.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		

	}

	void OnTriggerEnter(Collider other){
		Debug.Log (other);
		if (other.gameObject.tag == "Player"){
			CamSwap ();
		}
	}

	void CamSwap(){
		if (topCamera.activeSelf){
			standardCamera.SetActive (true);
			topCamera.SetActive (false);
		} else if (standardCamera.activeSelf){
			topCamera.SetActive (true);
			standardCamera.SetActive (false);
		}
	}
}
