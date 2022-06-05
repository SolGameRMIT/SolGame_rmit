using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerAgent_Demo4 : Agent
{

    public GameController_Demo4 aiGameController;
    GameObject RaceB;
    public GameObject Weapon;
    public float fileRate = 0.5f;
    float nextShootingTime = 0;
    float moveSpeed = 8f;
    float gameTime = 15f;
    float currentTime = 0f;
    List<GameObject> BlastList = new List<GameObject>();
    public void Awake()
    {
        Debug.Log("START 10.0");
    }

    public void Init(GameController_Demo4 controller)
    {
        aiGameController = controller;
        RaceB = controller.RaceB;
    }


    public override void OnEpisodeBegin()
    {
        nextShootingTime = 0;
        currentTime = 0f;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        BlastList.ForEach(b => Destroy(b));
        BlastList.Clear();
        aiGameController.GameReset();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        //Could I shoot?
        var shootTime = (Time.time - nextShootingTime) > 0 ? 0f : Time.time - nextShootingTime;
        sensor.AddObservation(shootTime);
        sensor.AddObservation(RaceB.GetComponent<RaceB_Demo4>().getHp());
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.002f);
        var forward = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        var horizontal = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);
        var canShoot = actions.DiscreteActions[0]; // 0 1
        var shootingAngle = actions.DiscreteActions[1]; // 01234567


        // move
        transform.localPosition += new Vector3(horizontal, 0, forward) * moveSpeed * Time.deltaTime;
        // shoot
        if (canShoot > 0 && Time.time > nextShootingTime)
        {
            Shoot(shootingAngle);
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boid"))
        {
            collision.gameObject.GetComponent<Boid_Demo4>().GameReset();
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
        if (collision.gameObject.CompareTag("Bullets"))
        {
            AddReward(-5f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("RaceB"))
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

    void Update()
    {

    }

    void FixedUpdate()
    {
        currentTime += Time.deltaTime;
        if(currentTime >= gameTime)
        {
            EndEpisode();
        }



    }

    void Shoot(int input)
    {
        var angle = 0f;
        switch (input)
        {
            case 0:
                angle = 0;
                break;
            case 1:
                angle = 45;
                break;
            case 2:
                angle = 90;
                break;
            case 3:
                angle = 135;
                break;
            case 4:
                angle = 180;
                break;
            case 5:
                angle = 225;
                break;
            case 6:
                angle = 270;
                break;
            case 7:
                angle = 315;
                break;
            default:
                break;
        }
        nextShootingTime = Time.time + fileRate;

        GameObject temp = Instantiate(Weapon);
        temp.transform.localPosition = transform.localPosition;
        temp.transform.localEulerAngles = Vector3.up * angle;
        temp.GetComponent<Blast_Demo4>().Init(this);
        temp.transform.parent = transform.parent;
        BlastList.Add(temp);
        Destroy(temp, 5f);
    }



}
