using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBottleShaker : MonoBehaviour {

	private Vector3 baseRotation;
	private bool baseRotSet = false;
	private RectTransform myTransform;

	public float shakesFor = 0.3f;
	private float shakeTimer;

	private bool timerSet = false;
	private bool shake = false;


	// Use this for initialization
	void Start () {
		
		myTransform = GetComponent<RectTransform> ();
		baseRotation = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		bam ();
		Shake ();
	}

	public void Shake(){
		if (shake) {
			if (!timerSet) {
				shakeTimer = Time.timeSinceLevelLoad;
				timerSet = true;
			}

			if (Time.timeSinceLevelLoad - shakeTimer < shakesFor) {
				float rot = Mathf.PingPong (Time.time * 200, 15f);
				Vector3 myRot = new Vector3 (baseRotation.x, baseRotation.y, baseRotation.z + rot);
				transform.rotation = Quaternion.Euler (myRot);
			} else {
				timerSet = false;
				shake = false;
				transform.rotation = Quaternion.Euler (Vector3.zero);
			}
		}
	}

	private void bam(){
		if (Input.GetKeyDown(KeyCode.Space)){
			shake = true;
		}
	}
}
