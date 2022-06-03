using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController_Demo3 : MonoBehaviour
{
    public GameObject Boid;
    public GameObject RaceA;
    public GameObject RaceB;
    public GameObject Box;
    GameObject box1;
    GameObject box2;
    GameObject RaceB1;
    GameObject PlayerAgent;
    GameObject Court;
    List<GameObject> BoidList = new List<GameObject>();
    List<GameObject> RaceAList = new List<GameObject>();
    List<GameObject> BoxList = new List<GameObject>();
    float timeSpend = 0f;
    // Start is called before the first frame update
    void Start()
    {
        PlayerAgent = gameObject.transform.parent.Find("Agent_Demo3").gameObject;
        Court = gameObject.transform.parent.Find("Court").gameObject;
        GameObjectInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameObjectInit()
    {

        while (BoidList.Count <= 4)
        {
            var target = Instantiate(Boid);
            target.GetComponent<Boid_Demo3>().Init(PlayerAgent, this);
            target.transform.parent = transform.parent;
            target.transform.localPosition = new Vector3(Random.Range(-15, 15), 0.5f, Random.Range(-15, 15));
            target.name = "Boid";
            BoidList.Add(target.gameObject);
        }

        while (RaceAList.Count <= 2)
        {
            var target = Instantiate(RaceA);
            target.GetComponent<RaceA_Demo3>().Init(PlayerAgent, this);
            target.transform.parent = transform.parent;
            if (Random.Range(0f, 2f) > 1)
            {
                target.transform.localPosition = new Vector3(Random.Range(-15, -10), 1, Random.Range(-15, 15));
            }
            else
            {
                target.transform.localPosition = new Vector3(Random.Range(10, 15), 1, Random.Range(-15, 15));
            }
            target.name = "RaceA";
            RaceAList.Add(target.gameObject);
        }

        RaceB1 = Instantiate(RaceB);
        RaceB1.GetComponent<RaceB_Demo3>().Init(PlayerAgent, this);
        RaceB1.transform.parent = transform.parent;
        RaceB1.transform.localPosition = new Vector3(Random.Range(-2, 2), 0, Random.Range(-15, -10));
        RaceB1.name = "RaceB1";

        box1 = Instantiate(Box);
        box2 = Instantiate(Box);
        box1.transform.parent = Court.gameObject.transform;
        box2.transform.parent = Court.gameObject.transform;
        box1.transform.localPosition = new Vector3(Random.Range(-8, -2), 1, Random.Range(-6, 6));
        box2.transform.localPosition = new Vector3(Random.Range(2, 8), 1, Random.Range(-6, 6));
        box2.transform.localPosition = new Vector3(Random.Range(2, 8), 1, Random.Range(-6, 6));
        box1.name = "box1";
        box2.name = "box2";

    }

    private void FixedUpdate()
    {
        timeSpend +=Time.deltaTime;
    }

    public void GameReset()
    {
        box1.transform.localPosition = new Vector3(Random.Range(-8, -2), 1, Random.Range(-6, 6));
        box2.transform.localPosition = new Vector3(Random.Range(2, 8), 1, Random.Range(-6, 6));
        RaceB1.transform.localPosition = new Vector3(Random.Range(-2, 2), 0, Random.Range(-15, -10));
        BoidList.ForEach(target => target.GetComponent<Boid_Demo3>().GameReset());
        RaceAList.ForEach(target => target.GetComponent<RaceA_Demo3>().GameReset());
    }

}
