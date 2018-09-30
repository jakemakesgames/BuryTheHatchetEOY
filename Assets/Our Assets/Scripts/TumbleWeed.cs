using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleWeed : MonoBehaviour {

    public float textureAlpha = 1f;
    public float moveSpeed = 10f;

    private GameObject mesh;
    private GameObject spawnPos;
    private GameObject goalPos;

    private bool triggered = false;


	void Start () {


        //Setup Variables inside Parent Object
        Transform[] childObjs = GetComponentsInChildren<Transform>();

        for (int i = 0; i < childObjs.Length; i++) {

            if (childObjs[i].name == "Spawn Position") {
                spawnPos = childObjs[i].gameObject;
            } else if (childObjs[i].name == "Mesh") {
                mesh = childObjs[i].gameObject;
                mesh.transform.position = spawnPos.transform.position;
                mesh.SetActive(false);
            } else if (childObjs[i].name == "Goal Position") {
                goalPos = childObjs[i].gameObject;
            }

        }

    }
	

	void Update () {
        Move();
	}



    public void Spawn() {

    }

    void Move() {
        if (triggered) {

        }
    }
}
