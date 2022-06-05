using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceB_Demo3 : MonoBehaviour
{

    GameController_Demo3 gameController;
    GameObject PlayerAgent;
    float moveSpeed;
    // Start is called before the first frame update

    void Start()
    {

    }
    public void Init(GameObject agent, GameController_Demo3 controller)
    {
        PlayerAgent = agent;
        gameController = controller;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 deltaVec = PlayerAgent.transform.position - transform.position;
        deltaVec.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(deltaVec);
        transform.rotation = rotation;
    }
    public void GameReset()
    {

    }
}
