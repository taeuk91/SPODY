using System;
using System.Collections;
using System.Collections.Generic;
using MirzaBeig.Demos.TheLastParticle;
using UnityEngine;
using Random = UnityEngine.Random;


namespace SPODY_6_7_3
{
    public class CharacterMove : MonoBehaviour
    {
        [SerializeField] bool isClockWise = true;
        [SerializeField] [Range(0f, 10f)] private float speed = 1;
        [SerializeField] [Range(0f, 10f)] private float radius = 1;
        [SerializeField] [Range(0f, 100f)] private float rotSpeed = 1;
        [SerializeField] [Range(0f, 10f)] private float rotDuration = 1;
        [SerializeField] float rotZAngle;
        [SerializeField] bool togle = true;
        private float runningTime = 0;
        float currentTime = 0;
        private Vector3 newPos = new Vector3();
        [SerializeField] [Range(0f, 10f)] float moveSpeed = 1;
        [SerializeField] private bool isRight = true;

        [SerializeField] private Transform rightPos;
        [SerializeField] private Transform leftPos;
        private Vector3 dir;

        private void OnEnable()
        {
            int rand = Random.Range(0, 2);
            switch (rand)
            {
                case 0:
                    isRight = isRight;
                    break;
                case 1:
                    isRight = !isRight;
                    break;
            }
        }

        private void Update()
        {
            UpAndDown();

            Move();

            Rotate();
        }

        void UpAndDown()
        {
            runningTime += Time.deltaTime * speed;
            float y = radius * Mathf.Sin(runningTime);
            newPos = new Vector3(0, y, 0);
        }

        void Move()
        {
            switch (isRight)
            {
                case true:
                    //print("오른쪽!");
                    dir = (rightPos.position - transform.position) + newPos;
                    break;
                case false:
                    //print("왼쪽!");
                    dir = (leftPos.position - transform.position) + newPos;
                    break;
            }

            float distance = dir.magnitude;

            if (distance < 0.3f)
            {
                isRight = !isRight;
                //this.GetComponent<SpriteRenderer>().flipX = !this.GetComponent<SpriteRenderer>().flipX;
            }

            this.transform.localPosition += dir.normalized * (moveSpeed * Time.deltaTime);
        }

        void Rotate()
        {
            currentTime += Time.deltaTime;
            if (currentTime > rotDuration)
            {
                currentTime = 0;
                togle = !togle;
            }

            if (togle)
            {
                //print("시계방향 회전");
                rotZAngle += rotSpeed * Time.deltaTime;
            }
            else
            {
                //print("반시계방향 회전");
                rotZAngle -= rotSpeed * Time.deltaTime;
            }

            float distance = dir.magnitude;

            float nowDegree;

            if (isRight)
            {
                nowDegree = 0;
            }
            else
            {
                nowDegree = -180;
            }

            this.transform.rotation = Quaternion.Euler(0, nowDegree, rotZAngle);

        }
    }
}