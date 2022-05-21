using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class DroidAgent : Agent
{
    private Rigidbody rb;

    //private Vector3 droidStartPos;

    //[SerializeField] private Transform coinsParent;
    //[SerializeField] private Transform bombsParent;
    [SerializeField] private GameObject target, obstacle;
    [SerializeField] private GameObject bullet;
    private List<GameObject> targets = new List<GameObject>();
    private List<GameObject> obstacles = new List<GameObject>();
    private float nextFire = 0f;
    private float fireRate = 1f;
    private float randomRange = 15f;
    private float rotateSpeed = 2f;
    private float thrust = 1f;
    private int targetsCollectedThisEp;
    private float acceleration ;
    private float accDrag = 0.9f;
    private float maxSpeed = 10f;
    private int CurrentStep = 0;
    private float ShootPenalty = -1f;
    private int numOfObstacles = 10 , numOfTargets = 15;
    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;

        MaxStep = 2500;

        // Get and store a references to the Rigidbody2D and SpriteRenderer components.
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;
        transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
        //foreach (Transform coin in coinsParent)
        //{
        //    coin.gameObject.SetActive(true);
        //    coin.transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
        //}

        //foreach (Transform bomb in bombsParent)
        //{
        //    bomb.gameObject.SetActive(true);
        //    bomb.transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
        //}
        for (int i = 0; i < numOfTargets; i++)
        {
            var newSpawn = Instantiate(target, Vector3.zero, Quaternion.identity);
            newSpawn.transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
            targets.Add(newSpawn);
            newSpawn.SetActive(true);
        }
        for (int i = 0; i < numOfObstacles; i++)
        {
            var newSpawn = Instantiate(obstacle, Vector3.zero, Quaternion.identity);
            newSpawn.transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
            obstacles.Add(newSpawn);
            newSpawn.SetActive(true);
        }
    }
    public void HandleCollectTarget()
    {
        AddReward(1.0f);
        targetsCollectedThisEp++;
        //if (targetsCollectedThisEp == coinsParent.childCount)
        //{
        //    EndEpisode();
        //}
        if (targetsCollectedThisEp == numOfTargets)
        {
            EndEpisode();
        }
    }
    public void HandleShootTarget()
    {
        AddReward(5.0f);
        targetsCollectedThisEp++;
        //if (targetsCollectedThisEp == coinsParent.childCount)
        //{
        //    EndEpisode();
        //}
        if (targetsCollectedThisEp == numOfTargets)
        {
            EndEpisode();
        }
    }
    public void HandleHitObstacle()
    {
        AddReward(-10f);
    }
    public void HandleShootObstacle()
    {
        AddReward(-3f);
    }
    public override void OnEpisodeBegin()
    {
        Debug.Log("episode begin: " + CurrentStep);
        targetsCollectedThisEp = 0;
        this.CurrentStep += this.MaxStep;
        transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
        rb.velocity = Vector3.zero;
        //foreach (Transform coin in coinsParent)
        //{
        //    coin.gameObject.SetActive(true);
        //    coin.transform.position =new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
        //}

        //foreach (Transform bomb in bombsParent)
        //{
        //    bomb.gameObject.SetActive(true);
        //    bomb.transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
        //}
        foreach(GameObject target in targets)
        {
            Destroy(target);
        }
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        for (int i = 0; i < numOfTargets; i++)
        {
            var newSpawn = Instantiate(target, Vector3.zero, Quaternion.identity);
            newSpawn.transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
            targets.Add(newSpawn);
            newSpawn.SetActive(true);
        }
        for (int i = 0; i < numOfObstacles; i++)
        {
            var newSpawn = Instantiate(obstacle, Vector3.zero, Quaternion.identity);
            newSpawn.transform.position = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
            obstacles.Add(newSpawn);
            newSpawn.SetActive(true);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
       //print("collect observation");
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(rb.velocity);
    }
    void Update()
    {
        Vector3 forceDirection = Vector3.zero;
        Vector3 rotateVector = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotateVector = transform.up * (-rotateSpeed);
        }
        else if(Input.GetKey(KeyCode.RightArrow))
        {
            rotateVector = transform.up * rotateSpeed;
        }
        else if(Input.GetKey(KeyCode.UpArrow))
        {
            acceleration += thrust;
            forceDirection = transform.forward * acceleration;
        }
        if(Input.GetKey(KeyCode.Space) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject temp = Instantiate(bullet, transform.position, Quaternion.Euler(rb.rotation.eulerAngles));
            temp.name = "Bolt Bullet";
            Destroy(temp, 8f);
            if(CurrentStep > 250000)
            {
                AddReward(ShootPenalty);
            }
        }


        rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles + rotateVector));
        rb.AddForce(forceDirection);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        acceleration *= accDrag;
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int action = actionBuffers.DiscreteActions[0];
        Vector3 forceDirection = Vector3.zero;
        Vector3 rotateVector = Vector3.zero;
        if (action == 0)
        {
            //pass
        }
        else if (action == 1)
        {
            rotateVector = transform.up * (-rotateSpeed);
        }
        else if (action == 2)
        {
            rotateVector = transform.up * rotateSpeed;
        }
        else if (action == 3)
        {
       
            acceleration += thrust;
            forceDirection = transform.forward * acceleration;
        }
        else if (action == 4 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject temp = Instantiate(bullet, transform.position, Quaternion.Euler(rb.rotation.eulerAngles));
            temp.name = "Bolt Bullet";
            Destroy(temp, 8f);
            if (CurrentStep > 250000)
            {
                AddReward(ShootPenalty);
            }
        }

        rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles + rotateVector));
        rb.AddForce(forceDirection);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        acceleration *= accDrag;

    }

}
