using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Demo2 : MonoBehaviour
{
    GameController_Demo2 gameController;
    GameObject PlayerAgent;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void init(GameObject agent, GameController_Demo2 controller)
    {
        PlayerAgent = agent;
        gameController = controller;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameReset()
    {
        transform.localPosition = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }



}
