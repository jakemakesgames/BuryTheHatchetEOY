﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMove : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] AnimationCurve animCurve;
    [SerializeField] Animation snakingAnim;
    [SerializeField] float animSpeed;
    [SerializeField] float speedMultiplier;
    Vector3 startPos;
    float moveSpeed;
    bool move = false;

    // Use this for initialization
    void Start()
    {
        snakingAnim["idle"].speed = animSpeed;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Reset();
        }
       
        if (move)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {

        Vector3 target = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
        gameObject.transform.LookAt(target);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target, moveSpeed * speedMultiplier);
        if (gameObject.transform.position == target)
        {
            move = false;
        }

    }

    private void Reset()
    {
        transform.position = startPos;
        move = false;
    }

    IEnumerator SpeedModifier()
    {
        while (move)
        {
            moveSpeed = animCurve.Evaluate(Mathf.Sin(Time.time * 12f) + 0.5f);
            yield return 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && move == false)
        {
            float rand = Random.value;
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            move = true;
            StartCoroutine(SpeedModifier());
        }

    }

}

