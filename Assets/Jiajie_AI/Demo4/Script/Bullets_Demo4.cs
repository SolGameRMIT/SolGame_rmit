using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets_Demo4 : MonoBehaviour
{
    PlayerAgent_Demo4 playerAgent;
    RaceB_Demo4 raceB;
    public float speed = 15f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(PlayerAgent_Demo4 Agent, RaceB_Demo4 RaceB)
    {
        playerAgent = Agent;
        raceB = RaceB;
    }

    private void FixedUpdate()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, playerAgent.transform.localPosition, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider == null)
        {
            return;
        }
        if (playerAgent == null)
        {
            return;
        }
        if (collider.gameObject.tag == "enemy")
        {
            collider.gameObject.SetActive(false);
            //collider.gameObject.GetComponent<RaceA_Demo4>().GameReset();
        }

        if (collider.gameObject.tag == "Boid")
        {
            collider.gameObject.SetActive(false);
            collider.gameObject.GetComponent<Boid_Demo4>().GameReset();
        }
        if (collider.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (collider.gameObject.tag == "Agent")
        {
            Destroy(gameObject);
            playerAgent.AddReward(-5f);
            playerAgent.EndEpisode();
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
