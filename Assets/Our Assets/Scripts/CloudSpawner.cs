using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour {

	public GameObject[] clouds;
	private float spawnTimer;
	private float timeBetweenClouds = 14f;

	// Use this for initialization
	void Start () {
		spawnTimer = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - spawnTimer > timeBetweenClouds){
			int whichCloud = Random.Range (0, clouds.Length);
			Vector3 spawnPos = transform.position;
			spawnPos.x += Random.Range (-50f, 50f);
			Instantiate(clouds[whichCloud],spawnPos,Quaternion.identity);
			spawnTimer = Time.timeSinceLevelLoad;
		}
	}
}
