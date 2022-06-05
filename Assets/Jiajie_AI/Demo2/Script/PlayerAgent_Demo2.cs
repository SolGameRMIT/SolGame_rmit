using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerAgent_Demo2 : Agent
{

    public GameController_Demo2 aiGameController;
    public GameObject Weapon;
    public float fileRate = 2;
    int shootCount = 0;
    float maxShootPenalty = 1.1f;
    float nextFile = 0;
    float moveSpeed = 10f;
    float turnSpeed = 300;
    public void Awake()
    {
        Debug.Log("Start! Demo 21");
    }

    public void Init(GameController_Demo2 controller)
    {
        aiGameController = controller;
    }


    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        shootCount = 0;
        nextFile = 0;
        aiGameController.GameReset();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //Could I shoot?
        sensor.AddObservation(Time.time > nextFile);
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        var forward = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var horizontal = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        var canShoot = actions.DiscreteActions[0]; // 0 1
        var shootingAngle = actions.DiscreteActions[1]; // 01234567


        // move
        transform.localPosition += new Vector3(forward,0, horizontal) * moveSpeed * Time.deltaTime;
        // shoot
        if (canShoot > 0 && Time.time > nextFile)
        {
            this.shootCount += 1;
            Shoot(shootingAngle);
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        GetCumulativeReward();
        if (collision.gameObject.CompareTag("Boid"))
        {
            collision.gameObject.GetComponent<Boid_Demo2>().GameReset();
            AddReward(1f);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-5f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("enemy"))
        {
            AddReward(-5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;

        // index = 0 : up down 
        if (Input.GetKey(KeyCode.W))
        {
            continuousActionsOut[0] = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            continuousActionsOut[0] = -1;
        }

        // index = 1 : roate left, rifght
        if (Input.GetKey(KeyCode.A))
        {
            continuousActionsOut[1] = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            continuousActionsOut[1] = 1;
        }

    }



    void FixedUpdate()
    {
    }

    void Shoot(int input)
    {
        nextFile = Time.time + fileRate;
        var angle = input * 45 - 180;
        GameObject temp = Instantiate(Weapon);
        temp.transform.localPosition = transform.localPosition;
        temp.transform.Rotate(transform.up * angle);
        temp.GetComponent<Blast_Demo2>().Init(this);
        temp.transform.parent = transform.parent;
        Destroy(temp, 5f);
    }



}
