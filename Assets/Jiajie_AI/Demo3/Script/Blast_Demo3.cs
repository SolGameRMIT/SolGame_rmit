using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast_Demo3 : MonoBehaviour
{

    PlayerAgent_Demo3 playerAgent;
    public float speed = 20f;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init(PlayerAgent_Demo3 Agent)
    {
        playerAgent = Agent;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(" transform.position " + transform.position);
    }

    private void FixedUpdate()
    {
        transform.localPosition += transform.forward * speed * Time.deltaTime;
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
            Debug.Log("Shooted at enemy AddReward 0.2 ");
            collider.gameObject.SetActive(false);
            collider.gameObject.GetComponent<RaceA_Demo3>().GameReset();
            playerAgent.AddReward(0.2f);
            Destroy(gameObject);
        }
        if (collider.gameObject.tag == "Wall")
        {
            Debug.Log("Shooted at Wall, AddReward -0.1 ");
            playerAgent.AddReward(-0.1f);
            Destroy(gameObject);
        }
        if (collider.gameObject.tag == "Boid")
        {
            Debug.Log("Shooted at Boid AddReward -1 ");
            collider.gameObject.SetActive(false);
            collider.gameObject.GetComponent<Boid_Demo3>().GameReset();
            playerAgent.AddReward(-1f);
            Destroy(gameObject);
        }
    }


    private void OnDestroy()
    {
    }

}
