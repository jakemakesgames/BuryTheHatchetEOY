using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenPause : MonoBehaviour {

	public GameObject title;

	public Animator deathAnim;

	// Use this for initialization
	void Start () {
		title.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad > 1.7f) {
		//if (Time.timeSinceLevelLoad > 1.71f) {
			//Time.timeScale = 0f;
			deathAnim.SetTrigger("KillTrigger");
			//title.SetActive (true);
		}
	}
}
