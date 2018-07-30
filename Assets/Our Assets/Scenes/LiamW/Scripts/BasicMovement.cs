using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{

	// Update is called once per frame
	void Update ()
    {
        float z = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;

        transform.Translate(0, 0, z);
	}
}
