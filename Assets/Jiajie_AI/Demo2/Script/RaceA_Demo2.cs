using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceA_Demo2 : MonoBehaviour
{

    GameController_Demo2 gameController;
    GameObject PlayerAgent;
    float moveSpeed;
    // Start is called before the first frame update

    void Start()
    {
        moveSpeed = Random.Range(4f, 7f);
    }
    public void Init(GameObject agent, GameController_Demo2 controller)
    {
        PlayerAgent = agent;
        gameController = controller;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, PlayerAgent.transform.localPosition, moveSpeed * Time.deltaTime);
    }
    public void GameReset()
    {
        moveSpeed = Random.Range(4f, 7f);
        transform.localPosition = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }
}
