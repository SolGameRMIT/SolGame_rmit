using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController_Demo4 : MonoBehaviour
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
    public Text TimeLeft;
    public float timeSpend = 0;
    List<GameObject> BoidList = new List<GameObject>();
    List<GameObject> RaceAList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        PlayerAgent = gameObject.transform.parent.Find("Agent_Demo4").gameObject;
        PlayerAgent.GetComponent<PlayerAgent_Demo4>().Init(this);
        Court = gameObject.transform.parent.Find("Court").gameObject;
        GameObjectInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameObjectInit()
    {

        while (BoidList.Count <= 7)
        {
            var target = Instantiate(Boid);
            target.GetComponent<Boid_Demo4>().Init(PlayerAgent, this);
            target.transform.parent = transform.parent;
            target.transform.localPosition = new Vector3(Random.Range(-15, 15), 0.5f, Random.Range(-15, 15));
            target.name = "Boid";
            BoidList.Add(target.gameObject);
        }

        while (RaceAList.Count <= 4)
        {
            var target = Instantiate(RaceA);
            target.GetComponent<RaceA_Demo4>().Init(PlayerAgent, this);
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
        RaceB1.GetComponent<RaceB_Demo4>().Init(PlayerAgent, this);
        RaceB1.transform.parent = transform.parent;
        RaceB1.transform.localPosition = new Vector3(0, 0, -10);
        RaceB1.name = "RaceB1";

        box1 = Instantiate(Box);
        box2 = Instantiate(Box);
        box1.transform.parent = Court.gameObject.transform;
        box2.transform.parent = Court.gameObject.transform;
        box1.transform.localPosition = new Vector3(Random.Range(-8, -2), 1, Random.Range(-6, 6));
        box2.transform.localPosition = new Vector3(Random.Range(2, 8), 1, Random.Range(-6, 6));
        box1.name = "box1";
        box2.name = "box2";

    }

    private void FixedUpdate()
    {
        timeSpend += Time.deltaTime;
        var temp = (int)(15 - timeSpend);
        TimeLeft.text = string.Format("Time:{0}",temp);
    }

    public void GameReset()
    {
        timeSpend = 0;
        box1.transform.localPosition = new Vector3(Random.Range(-8, -2), 1, Random.Range(-6, 6));
        box2.transform.localPosition = new Vector3(Random.Range(2, 8), 1, Random.Range(-6, 6));
        BoidList.ForEach(target => target.GetComponent<Boid_Demo4>().GameReset());
        RaceAList.ForEach(target => target.GetComponent<RaceA_Demo4>().GameReset());
        RaceB1.GetComponent<RaceB_Demo4>().GameReset();
    }

}
