using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinecartFlair : MonoBehaviour {

    [Header("Control Variables")]
    public bool isBraking = false;
    public float velocity = 0f;

    [Header("Flair Variables")]
    public float hullBounceHeight = 0.075f;
    public float hullBounceSpeed = 0.18f;
    public float wheelRotationSpeed = 50f;

    // ------------------------------- //

    private GameObject wheelsF;
    private GameObject wheelsB;
    private GameObject hull;
    private GameObject sparks;

    private Vector3 rotationAxis = new Vector3(1, 0, 0);

    // ------------------------------- //

    [Header("Plug In Variables")]
    public AudioSource movementSpeaker;
    public AudioSource brakeSpeaker;

    public AudioClip s_Movement;
    public AudioClip s_Braking;


    // Setup Variables
    void Start() {

        Transform[] childrenObjs = GetComponentsInChildren<Transform>();

        for (int i = 0; i < childrenObjs.Length; i++) {

            if (childrenObjs[i].name == "WheelsF") {
                wheelsF = childrenObjs[i].gameObject;
            }
            else if (childrenObjs[i].name == "WheelsB") {
                wheelsB = childrenObjs[i].gameObject;
            }
            else if (childrenObjs[i].name == "Hull") {
                hull = childrenObjs[i].gameObject;
            }
            else if (childrenObjs[i].name == "Sparks") {
                sparks = childrenObjs[i].gameObject;
            }

        }

        sparks.SetActive(false);

        //Assign sounds to audio sources
        movementSpeaker.clip = s_Movement;
        brakeSpeaker.clip = s_Braking;
    }
	
	void Update () {

        VelocityUpdate();
        BrakeUpdate();

	}

    //Moves the wheels, hull and plays audio based on velocity
    void VelocityUpdate() {

        if (velocity > 0) {

            //Hull
            hull.transform.position = new Vector3(transform.position.x, Mathf.PingPong(Time.time * hullBounceSpeed, hullBounceHeight), transform.position.z);

            //Wheels
            wheelsF.transform.Rotate(rotationAxis * (velocity * wheelRotationSpeed) * Time.deltaTime);
            wheelsB.transform.Rotate(rotationAxis * (velocity * wheelRotationSpeed) * Time.deltaTime);

            //Movement Audio
            if (!movementSpeaker.isPlaying) {
                movementSpeaker.Play();
            }
            
        } else {

            //Cancel Move Audio
            if (movementSpeaker.isPlaying) {
                movementSpeaker.Stop();
            }

        }

    }

    //Toggles the particles and plays audio based on braking
    void BrakeUpdate() {

        if (isBraking) {

            //Audio
            if (!brakeSpeaker.isPlaying) {
                brakeSpeaker.Play();
            }


            //Particles
            sparks.SetActive(true);

        } else {

            //Particles
            sparks.SetActive(false);

            //Audio
            if (brakeSpeaker.isPlaying) {
                brakeSpeaker.Stop();
            }

        }

    }
}
