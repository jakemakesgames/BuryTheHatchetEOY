using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCardTransforms : MonoBehaviour {

	public Transform activePosition;
	public Transform deactivePosition;
	public GameObject playingCard;

	private float speed = 800f;

	public bool activate = true;

    private bool rotate = false;
    private int spinR = 0; 

	// Use this for initialization
	void Start () {
	
		Transform[] children = GetComponentsInChildren<Transform> ();
		for (int i = 0; i < children.Length; i++) {
			if (children[i].name == "Active Position"){
				activePosition = children [i];
			}
			if (children[i].name == "Deactive Position"){
				deactivePosition = children [i];
			}
			if (children[i].name == "Playing Card Image"){
				playingCard = children [i].gameObject;
			}
		}

	}
	// Update is called once per frame
	void Update () {
		
		ActiveMove ();
		DeactiveMove();
        Rotate();
	
	}

	void ActiveMove(){
		float moveSpeed = speed * Time.deltaTime;
		if (activate){
			playingCard.transform.position = Vector3.MoveTowards (playingCard.transform.position, activePosition.position, moveSpeed);
		}
	}

	void DeactiveMove(){
		float moveSpeed = speed * Time.deltaTime;
		if (!activate){
			playingCard.transform.position = Vector3.MoveTowards (playingCard.transform.position, deactivePosition.position, moveSpeed);
		}
	}

    void Rotate() {
        if (rotate) {
            if (spinR == 0) {
                playingCard.transform.Rotate(0, 0, -5);
            } else if (spinR == 1) {
                playingCard.transform.Rotate(0, 0, 5);
            }
        }
    }

	public void UseCard(){
		if (activate) {
			activate = false;
            rotate = true;
            spinR = Random.Range(0, 2);
		}
	}

	public void RestoreCard(){
		if (!activate){
			activate = true;
            rotate = false;
            playingCard.transform.rotation = Quaternion.Euler(0,0,0);
		}
	}
}
