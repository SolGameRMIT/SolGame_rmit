using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Linq;

public class DroidAgent : Agent
{
    private Rigidbody rb;

    //private Vector3 droidStartPos;

    [SerializeField] private GameObject target, obstacle;
    [SerializeField] private Transform wallFolder;
    [SerializeField] private GameObject droidBolt, _playerBolt;

    private float nextFire = 0f;
    private float nextShield = 0f;
    private float fireRate = 1f;
    private float shieldRate = 10f;
    private float shieldDuration = 2f;
    private float shieldDt = 0f;
    private bool shieldActivated = false;
    private float randomRange = 17.5f;
    private float rotateSpeed = 2f;
    private int shieldSuccess = 0;
    private int hitByPlayerBolt = 0;
    private int numOfTimeHitObstacle = 0;
    private int numOfTimeShootObstacle = 0;
    private int numOfMissedShot = 0;
    private int numOfShot = 0;
    private float thrust = 2f;
    private int targetShotCount;
    private float acceleration;
    private float accDrag = 0.9f;
    private float velDrag = 0.5f;
    private float maxSpeed = 15f;
    private int CurrentStep = 0;
    private float ShotMissPenalty = -0.1f;
    private float maxTargetSpeed = 3f;
    private float maxObstacleSpeed = 0.5f;
    private int targetHP = 5;
    private int maxObstacle = 10;
    private float maxDistance = 10f;
    private float minDistance = 4f;
    private float HitObstaclePenalty = -8f;
    private float FollowPlayerReward = 0.02f;
    private float ShootObstaclePenalty = -4f;
    private float HitTargetReward = 2f;
    private float KillTargetReward = 10f;
    private float ShieldSuccessReward = 10f;
    private float HitByPlayerBolt = -1f;
    private float Braking = -0.7f;
    private float RewardGainedByFollow = 0f;
    private float ShieldWastedPenalty = -5f;
    private bool shieldWasted = true;
    private int numOfWastedShield = 0;
    private int numberOfBulletsShotAtDroid = 10;
    [SerializeField] private float timeBetweenShootingAtDroid;
    private List<GameObject> obstacles = new List<GameObject>();
    void Start()
    {
        //startPos = transform.position;
        MaxStep = 2500;

        // Get and store a references to the Rigidbody2D and SpriteRenderer components.
        rb = GetComponent<Rigidbody>();
        rb.drag = velDrag;
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;
        timeBetweenShootingAtDroid = Random.Range(10, 30);


    }
    public void HandleShieldSuccess()
    {
        shieldWasted = false;
        AddReward(ShieldSuccessReward);
        shieldSuccess++;
    }
    public void HandleHitByPlayerBolt()
    {
        AddReward(HitByPlayerBolt);
        hitByPlayerBolt++;
    }
    public void HandleMissShot()
    {
        numOfMissedShot++;
        AddReward(ShotMissPenalty);
    }

    public void HandleShootTarget()
    {
        AddReward(HitTargetReward);
        targetShotCount++;
        if (targetShotCount == targetHP)
        {
            AddReward(KillTargetReward);
            EndEpisode();
        }
    }
    public void HandleHitObstacle()
    {
        numOfTimeHitObstacle++;
        AddReward(HitObstaclePenalty);
    }
    public void HandleShootObstacle()
    {
        numOfTimeShootObstacle++;
        AddReward(ShootObstaclePenalty);
    }
    public override void OnEpisodeBegin()
    {
        //Debug.Log("episode begin: " + CurrentStep);
        targetShotCount = 0;
        Academy.Instance.StatsRecorder.Add("Number of Shot", numOfShot);
        Academy.Instance.StatsRecorder.Add("Number of Missed Shot", numOfMissedShot);
        Academy.Instance.StatsRecorder.Add("Number of Times Hit the Obstacle", numOfTimeHitObstacle);
        Academy.Instance.StatsRecorder.Add("Number of Times Shoot the Obstacle", numOfTimeShootObstacle);
        Academy.Instance.StatsRecorder.Add("Amount of Reward gained by following the Target", RewardGainedByFollow);
        Academy.Instance.StatsRecorder.Add("Number of Times Shield Success", shieldSuccess);
        Academy.Instance.StatsRecorder.Add("Number of Times Hit by Player Bolt", hitByPlayerBolt);
        Academy.Instance.StatsRecorder.Add("Number of Wasted Shields", numOfWastedShield);
        numOfWastedShield = 0;
        shieldSuccess = 0;
        hitByPlayerBolt = 0;
        numOfTimeHitObstacle = 0;
        numOfTimeShootObstacle = 0;
        numOfShot = 0;
        numOfMissedShot = 0;
        RewardGainedByFollow = 0;
        this.CurrentStep += this.MaxStep;

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localPosition = Vector3.zero;
        transform.localPosition = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));

        rb.velocity = Vector3.zero;

        target.transform.localPosition = new Vector3(0f, 0f, 5f);
        target.transform.localPosition = new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange));
        if (this.CurrentStep > 500000 * 4)
        {
            target.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange)), maxTargetSpeed);
            ShotMissPenalty = -0.2f;
            // HitByPlayerBolt = -1f;
            // ShieldSuccessReward = 5f;
        }

        foreach (GameObject obj in obstacles)
        {
            Destroy(obj);
        }
        obstacles.Clear();


        for (int i = 0; i < maxObstacle + (int)Mathf.Floor(this.CurrentStep / 1000000); i++)
        {
            var newObstacle = Instantiate(obstacle, this.transform.parent.localPosition + new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange)), Quaternion.identity);
            if (this.CurrentStep > 500000 * 6)
            {
                newObstacle.GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(new Vector3(Random.Range(-randomRange, randomRange), 0f, Random.Range(-randomRange, randomRange)), maxObstacleSpeed);
            }
            newObstacle.SetActive(true);
            obstacles.Add(newObstacle);
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //print("collect observation");

        sensor.AddObservation(this.transform.localPosition.x);
        sensor.AddObservation(this.transform.localPosition.z);
        sensor.AddObservation(this.GetComponent<Rigidbody>().velocity.x);
        sensor.AddObservation(this.GetComponent<Rigidbody>().velocity.z);
        sensor.AddObservation(Vector3.Distance(this.transform.localPosition, target.transform.localPosition));


    }
    void Update()
    {
        // Vector3 forceDirection = Vector3.zero;
        // Vector3 rotateVector = Vector3.zero;
        // if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     rotateVector = transform.up * (-rotateSpeed);
        // }
        // else if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     rotateVector = transform.up * rotateSpeed;
        // }
        // else if (Input.GetKey(KeyCode.UpArrow))
        // {
        //     acceleration += thrust;
        // }
        // else if (Input.GetKey(KeyCode.DownArrow))
        // {
        //     rb.AddForce(Braking * rb.velocity);
        // }
        // else if (Input.GetMouseButton(0) && Time.time > nextFire)
        // {
        //     nextFire = Time.time + fireRate;
        //     GameObject temp = Instantiate(droidBolt, transform.position, Quaternion.Euler(rb.rotation.eulerAngles));
        //     temp.name = "Droid Bolt";
        //     numOfShot++;
        //     Destroy(temp, 10f);
        // }
        // else if (Input.GetMouseButton(1) && Time.time > nextShield)
        // {
        //     nextShield = Time.time + shieldRate;
        //     shieldDt = Time.time;
        //     shieldActivated = true;
        //     transform.Find("Shield").gameObject.SetActive(true);
        // }

        // if (Vector3.Distance(this.transform.localPosition, target.transform.localPosition) <= maxDistance && Vector3.Distance(this.transform.localPosition, target.transform.localPosition) >= minDistance)
        // {
        //     AddReward(FollowPlayerReward);
        // }
        // if (shieldActivated)
        // {
        //     if (Time.time > shieldDt + shieldDuration)
        //     {
        //         shieldActivated = false;
        //         transform.Find("Shield").gameObject.SetActive(false);
        //     }
        // }
        // forceDirection = transform.forward * acceleration;
        // rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles + rotateVector));
        // rb.AddForce(forceDirection);
        // rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        // acceleration *= accDrag;


    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {


        int movingAction = actionBuffers.DiscreteActions[0]; //movement
        int abilityAction = actionBuffers.DiscreteActions[1]; //shoot and abilities
        Vector3 forceDirection = Vector3.zero;
        Vector3 rotateVector = Vector3.zero;
        if (movingAction == 0)
        {
            //pass
        }
        else if (movingAction == 1)
        {
            rotateVector = transform.up * (-rotateSpeed);
        }
        else if (movingAction == 2)
        {
            rotateVector = transform.up * rotateSpeed;
        }
        else if (movingAction == 3)
        {
            acceleration += thrust;
        }
        else if (movingAction == 4)
        {
            rb.AddForce(Braking * rb.velocity);
        }

        if (abilityAction == 0 && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject temp = Instantiate(droidBolt, transform.position, Quaternion.Euler(rb.rotation.eulerAngles));
            temp.name = "Droid Bolt";
            numOfShot++;
            Destroy(temp, 10f);
        }
        else if (abilityAction == 1 && Time.time > nextShield)
        {
            nextShield = Time.time + shieldRate;
            shieldDt = Time.time;
            shieldActivated = true;
            transform.Find("Shield").gameObject.SetActive(true);
        }

        forceDirection = transform.forward * acceleration;
        rb.MoveRotation(Quaternion.Euler(rb.rotation.eulerAngles + rotateVector));
        rb.AddForce(forceDirection);
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        acceleration *= accDrag;
        if (Time.time > timeBetweenShootingAtDroid)
        {
            for (int i = 0; i < numberOfBulletsShotAtDroid; i++)
            {
                var playerBolt = Instantiate(_playerBolt, target.transform.position, Quaternion.LookRotation(transform.position + new Vector3(Random.Range(-randomRange, randomRange), 0, Random.Range(-randomRange, randomRange)) - target.transform.position, target.transform.up));
                playerBolt.name = "Player Bolt";
                Destroy(playerBolt, 10f);
            }
            timeBetweenShootingAtDroid = Time.time + Random.Range(10, 30);
        }
        if (Vector3.Distance(this.transform.localPosition, target.transform.localPosition) <= maxDistance && Vector3.Distance(this.transform.localPosition, target.transform.localPosition) >= minDistance)
        {
            AddReward(FollowPlayerReward);
            RewardGainedByFollow += FollowPlayerReward;
        }

        if (shieldActivated)
        {
            if (Time.time > shieldDt + shieldDuration)
            {
                shieldActivated = false;
                transform.Find("Shield").gameObject.SetActive(false);
                if (shieldWasted)
                {
                    AddReward(ShieldWastedPenalty);
                    numOfWastedShield++;
                }
                shieldWasted = true;
            }
        }

    }
}
