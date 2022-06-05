using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceA_Demo4 : MonoBehaviour
{

    GameController_Demo4 gameController;
    GameObject PlayerAgent;
    float moveSpeed;
    // Start is called before the first frame update

    void Start()
    {
        moveSpeed = Random.Range(6f, 8f);
    }
    public void Init(GameObject agent, GameController_Demo4 controller)
    {
        PlayerAgent = agent;
        gameController = controller;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, PlayerAgent.transform.localPosition, moveSpeed * Time.deltaTime);
        var temp = transform.localPosition;
        temp.y = 1f;
        transform.localPosition = temp;
    }
    public void GameReset()
    {

        moveSpeed = Random.Range(6f, 8f);

        if (Random.Range(0f, 2f) > 1)
        {
            transform.localPosition = new Vector3(Random.Range(-15, -10), 1, Random.Range(-15, 15));
        }
        else
        {
            transform.localPosition = new Vector3(Random.Range(10, 15), 1, Random.Range(-15, 15));
        }

        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }
}
