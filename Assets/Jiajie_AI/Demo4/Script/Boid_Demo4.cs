using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid_Demo4 : MonoBehaviour
{
    GameController_Demo4 gameController;
    GameObject PlayerAgent;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(GameObject agent, GameController_Demo4 controller)
    {
        PlayerAgent = agent;
        gameController = controller;
    }

    private void FixedUpdate()
    {
        var temp = transform.localPosition;
        temp.y = 0.5f;
        transform.localPosition = temp;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void GameReset()
    {
        var count = 0;
        var postion = Vector3.zero;
        while (count < 5)
        {
            postion = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
            var result = Physics.OverlapBox(transform.localPosition, Vector3.one * 3);
            foreach (var item in result)
            {
                if (item.gameObject.tag == "Boid" || item.gameObject.tag == "enemy" || item.gameObject.tag == "Wall" || item.gameObject.tag == "Agent")
                {
                    count++;
                    continue;
                }
            }
            break;
        }
        transform.localPosition = postion;
        transform.localRotation = Quaternion.identity;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.SetActive(true);
    }



}
