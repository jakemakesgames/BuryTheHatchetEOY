using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour {

	private float cloudSpeed = 5f;

	// Use this for initialization
	void Start () {
		cloudSpeed = Random.Range (0.5f, 1.5f);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += Vector3.forward * cloudSpeed * Time.deltaTime;
	}
}
