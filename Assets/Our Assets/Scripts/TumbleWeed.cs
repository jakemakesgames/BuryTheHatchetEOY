using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumbleWeed : MonoBehaviour {

    private float textureAlpha = 0f;
    public float moveSpeed = 10f;

    private GameObject mesh;
    private GameObject spawnPos;
    private GameObject goalPos;

    private float finalFadeStartDistance = 1f;

    private bool triggered = false;
    private bool fade = false;


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
        FadeDeathTrigger();
        FadeDeath();
    }

    //Trigger Mesh spawning 
    public void Spawn() {
        mesh.SetActive(true);
        triggered = true;
    }

    //Increase shader transparency & move tumbleweed towards goal position
    void Move() {
        if (triggered) {
            if (textureAlpha < 1) {
                textureAlpha += 0.1f;
            }

            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, goalPos.transform.position, step);
        }
    }

    //Detects distance to begin fading the tumbleweed
    void FadeDeathTrigger() {
        if (Vector3.Distance(goalPos.transform.position, transform.position) < finalFadeStartDistance) {
            fade = true;
        }
    }

    //Fades the tumbleweeds shader and destroys upon reaching minimum
    void FadeDeath() {
        if (fade && textureAlpha > 0) {
            textureAlpha -= 0.1f;
        }

        if (textureAlpha <= 0) {
            Destroy(this.gameObject);
        }
    }
}
