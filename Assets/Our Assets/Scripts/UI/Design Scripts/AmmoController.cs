using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour {

	public GameObject[] ammoCards;

	private int currentCard = 5;
    
	// Use this for initialization
	void Start () {
		PlayingCardTransforms[] cardTransforms = GetComponentsInChildren<PlayingCardTransforms>();

		for (int i = 0; i < cardTransforms.Length; i++) {
			ammoCards[i] = cardTransforms [i].gameObject;
		}
		
	}

    public void Shoot(){
		ammoCards[currentCard].GetComponent<PlayingCardTransforms> ().UseCard ();

		if (currentCard >= 0) {
			currentCard -= 1;
		}
	}

	public void Reload(){
        Debug.Log(currentCard.ToString());
        if (currentCard < ammoCards.Length -1) {
		    ammoCards [currentCard + 1].GetComponent<PlayingCardTransforms> ().RestoreCard ();
            currentCard++;
        }
	}
}
