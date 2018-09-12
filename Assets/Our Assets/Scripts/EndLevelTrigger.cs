using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour {

    public GameObject endLevel;

    private void Start()
    {
        endLevel.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"){
            endLevel.SetActive(true);
        }
    }
}
