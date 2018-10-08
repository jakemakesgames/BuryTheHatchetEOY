using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour {

	public GameObject[] ammoCards;

	private int currentCard = 5;

	//public float timeBetweenCardsUp = 1f;
	private float timer;

	private bool reloading = false;

	// Use this for initialization
	void Start () {
		timer = Time.time;

		PlayingCardTransforms[] cardTransforms = GetComponentsInChildren<PlayingCardTransforms>();

		for (int i = 0; i < cardTransforms.Length; i++) {
			ammoCards[i] = cardTransforms [i].gameObject;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Shoot(){
		ammoCards[currentCard].GetComponent<PlayingCardTransforms> ().UseCard ();

		if (currentCard != 0) {
			currentCard -= 1;
		}
	}

	public void Reload(){
		//for (int i = ammoCards.Length; i > 0;) {
		ammoCards [currentCard].GetComponent<PlayingCardTransforms> ().RestoreCard ();
		if (currentCard < ammoCards.Length - 1) {
			currentCard ++;
		}
	}
}
