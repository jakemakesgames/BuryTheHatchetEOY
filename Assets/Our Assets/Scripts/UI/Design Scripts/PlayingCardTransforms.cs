using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCardTransforms : MonoBehaviour {

	public Transform activePosition;
	public Transform deactivePosition;
	public GameObject playingCard;

	private float speed = 1750f;

	public bool activate = true;

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

	public void UseCard(){
		if (activate) {
			activate = false;
		}
	}

	public void RestoreCard(){
		if (!activate){
			activate = true;
		}
	}
}
