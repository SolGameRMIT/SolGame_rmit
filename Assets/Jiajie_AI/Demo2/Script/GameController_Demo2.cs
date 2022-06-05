using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController_Demo2 : MonoBehaviour
{
    public GameObject Boid;
    public GameObject RaceA;
    public GameObject Box;
    GameObject box1;
    GameObject box2;
    GameObject PlayerAgent;
    GameObject Court;
    List<GameObject> BoidList = new List<GameObject>();
    List<GameObject> RaceAList = new List<GameObject>();
    List<GameObject> BoxList = new List<GameObject>();
    public float timeSpend = 0f;
    // Start is called before the first frame update
    void Start()
    {
        PlayerAgent = gameObject.transform.parent.Find("AiAgent").gameObject;
        Court = gameObject.transform.parent.Find("Court").gameObject;
        GameObjectInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameObjectInit()
    {

        while (BoidList.Count <= 5)
        {
            var target = Instantiate(Boid);
            target.GetComponent<Boid_Demo2>().init(PlayerAgent, this);
            target.transform.parent = transform.parent;
            target.transform.localPosition = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
            target.name = "Target";
            BoidList.Add(target.gameObject);
        }


        while (RaceAList.Count <= 2)
        {
            var target = Instantiate(RaceA);
            target.GetComponent<RaceA_Demo2>().Init(PlayerAgent, this);
            target.transform.parent = transform.parent;
            target.transform.localPosition = new Vector3(Random.Range(-15, 15), 1, Random.Range(-15, 15));
            target.name = "RaceA";
            RaceAList.Add(target.gameObject);
        }


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
        timeSpend +=Time.deltaTime;
    }

    public void GameReset()
    {
        box1.transform.localPosition = new Vector3(Random.Range(-8, -2), 1, Random.Range(-6, 6));
        box2.transform.localPosition = new Vector3(Random.Range(2, 8), 1, Random.Range(-6, 6));
        BoidList.ForEach(target => target.GetComponent<Boid_Demo2>().GameReset());
        RaceAList.ForEach(target => target.GetComponent<RaceA_Demo2>().GameReset());
        var BlastArrayer = FindObjectsOfType<BlastMover>();
        foreach (var blast in BlastArrayer)
        {
            Destroy(blast.gameObject);
        }
        timeSpend = 0f;
    }

}
