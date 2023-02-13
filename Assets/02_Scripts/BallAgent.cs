using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System;

public class BallAgent : Agent
{
    private Transform tr;
    private Rigidbody2D rb;

    public Transform[] targetTr;
    public Transform wallTr_top;
    public Transform wallTr_right;
    public Transform wallTr_left;
    public Transform wallTr_bot;

    public GameObject bubbleboard;
    public GameObject arrowScript;

    public Vector2 startPosition;
    public int gameOverCnt;
    public int bubbleCnt;
    public bool canShoot;
    public bool canLook;
    public float eulerZAngle;

    private int moveCnt;

    //초기화 작업을 위해 한번 호출되는 메소드
    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        bubbleboard.GetComponent<BubbleSpawn>().StartSpawn();
        startPosition = tr.position;
        for(int j = 0; j < bubbleboard.transform.childCount; j++)
        {
            targetTr[j] = bubbleboard.transform.GetChild(j);
        }
    }

    public override void OnEpisodeBegin()
    {
        for(int i = 0; i < bubbleboard.transform.childCount; i++)
        {
            bubbleboard.transform.GetChild(i).gameObject.SetActive(true);
            
            Vector2 basePosition = bubbleboard.transform.position;
            float posX = basePosition.x + UnityEngine.Random.Range(-1250/2f, 1250/2f);
            float posY = basePosition.y + UnityEngine.Random.Range(-720/2f, 720/2f);

            bubbleboard.transform.GetChild(i).position = new Vector2(posX, posY);
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0;
        rb.gravityScale = 0;
        tr.position = startPosition;
        gameOverCnt = 0;
        bubbleCnt = 0;
        eulerZAngle = -90;
        moveCnt = 0;
        canShoot = false;
        canLook = true;
        arrowScript.SetActive(true);
    }
    
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        for(int i = 0; i < bubbleboard.transform.childCount; i++)
        {
            sensor.AddObservation(targetTr[i].localPosition);
        }
        sensor.AddObservation(tr.localPosition);
        sensor.AddObservation(wallTr_top.localPosition);
        sensor.AddObservation(wallTr_right.localPosition);
        sensor.AddObservation(wallTr_left.localPosition);
        sensor.AddObservation(wallTr_bot.localPosition);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float KeyAxis = 5 * Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        int Shooooting = actions.DiscreteActions[0];

        if (canLook)
        {
            eulerZAngle += KeyAxis;
            tr.eulerAngles = new Vector3(0, 0, eulerZAngle);
        }

        if (canShoot)
        {
            if (Shooooting == 1)
            {
                canShoot = false;
                canLook = false;
                arrowScript.SetActive(false);
                rb.velocity = tr.right * 1000.0f;
                rb.gravityScale = 75;
            }
        }

        if (moveCnt <= 50)
        {
            AddReward(0.01f);
        }
        if (moveCnt == 50)
        {
            canShoot = true;
        }
        moveCnt++;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag == "Bomb")
        {
            AddReward(1.0f);
            bubbleCnt++;
            if (bubbleCnt >= 30)
            {
                AddReward(10);
                EndEpisode();
            }
        }

        if(col.gameObject.tag == "Bubble")
        {
            AddReward(1.0f);
            bubbleCnt++;
            if (bubbleCnt >= 30)
            {
                AddReward(10);
                EndEpisode();
            }
        }

        if(col.gameObject.name == "Wall_Bottom")
        {
            if (gameOverCnt >= 10)
            {
                EndEpisode();
            }
            AddReward(-0.5f);
            gameOverCnt++;
        }

        if(col.gameObject.name == "Wall_GameEnd")
        {
            SetReward(0);
            EndEpisode();
        }
    }
}