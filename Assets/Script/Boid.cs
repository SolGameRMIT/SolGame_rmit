using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    static public List<Boid> boids;
    BoidSpawner boidSpawner;
    public Vector3 velocity;
    public Vector3 newVelocity;
    public Vector3 newPosition;

    float randomStrength = 2.0f;

    float crashRadius = 2.0f;
    float swarmRadius = 32.0f;

    public Boid closest;

    private void Awake()
    {

        var _gameController = GameObject.Find("GameController");
        if (_gameController != null)
        {
            boidSpawner = _gameController.GetComponent<BoidSpawner>();
        }


        if (boids == null)
            boids = new List<Boid>();


        boids.Add(this);

        // Place our boid in a random position
        Vector3 randPos = Random.insideUnitSphere * boidSpawner.spawnRadius;
        randPos.y = 1;
        this.transform.position = randPos;


        // Give them a random initial velocity
        velocity = Random.onUnitSphere;
        velocity *= boidSpawner.spawnVelcoty;


        Color randColor = Color.black;

        while (randColor.r + randColor.g + randColor.b < 1.0f)
            randColor = new Color(Random.value, Random.value, Random.value);

        Renderer[] rends = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
            r.material.color = randColor;
    }

    private void Update()
    {
        List<Boid> neighbours = Getneighbours(this);
        List<Boid> crashRisks = GetCrashRisks(this);

        newVelocity = velocity;
        newPosition = this.transform.position;


        Vector3 randVelocity = Random.onUnitSphere;
        newVelocity += randVelocity * boidSpawner.randomAmt;

        // Try move with a uniform pattern
        Vector3 neighbourVel = GetAverageVelocity(neighbours);
        newVelocity += neighbourVel * boidSpawner.velocityMatchingAmt;

        // Try move towards the same goal
        Vector3 neighbourCenterOffset = GetAveragePosition(neighbours) - this.transform.position;
        newVelocity += neighbourCenterOffset * boidSpawner.flockCenteringAmt;

        // Avoid each other
        Vector3 dist;
        if (crashRisks.Count > 0)
        {
            Vector3 averageCrashVec = GetAveragePosition(crashRisks);
            dist = averageCrashVec - this.transform.position;
            newVelocity -= dist * boidSpawner.crashAvoidanceAmt;
        }

        // Aim at the player, if they're alive!!!!
        if (boidSpawner.master)
        {
            dist = boidSpawner.master.transform.position - this.transform.position;
            newVelocity += dist.normalized;
        }

    }

    private void LateUpdate()
    {
        // Update our boid's position based on the velocity we calculated
        velocity = (1 - boidSpawner.velocityLerpAmt) * velocity + boidSpawner.velocityLerpAmt * newVelocity;

        // Check if we're above max velocity
        if (velocity.magnitude > boidSpawner.maxVelocity)
            velocity = velocity.normalized * boidSpawner.maxVelocity;
        if (velocity.magnitude < boidSpawner.minVelocity)
            velocity = velocity.normalized * boidSpawner.minVelocity;

        newPosition = this.transform.position + velocity * Time.deltaTime;
        newPosition.y = 0.0f;

        this.transform.LookAt(newPosition);

        this.transform.position = newPosition; ;

    }

    public List<Boid> Getneighbours(Boid boi)
    {
        List<Boid> neighbours = new List<Boid>();

        Vector3 delta;
        float dist;

        neighbours.Clear();

        foreach (Boid b in boids)
        {
            if (b == boi || !b || !boi) // Skip if we're matching ourselves or dead
                continue;

            delta = b.transform.position - boi.transform.position;
            dist = delta.magnitude;

            if (dist < swarmRadius) // Are they in our radius
                neighbours.Add(b);
        }
        //Debug.Log("Neighbours: "+neighbours.Count);
        return (neighbours);
    }
    public List<Boid> GetCrashRisks(Boid boi)
    {
        List<Boid> crashRisks = new List<Boid>();
        Vector3 delta;
        float dist;

        crashRisks.Clear();

        foreach (Boid b in boids)
        {
            if (b == boi || !b || !boi) // Skip if we're matching ourselves or dead
                continue;

            delta = b.transform.position - boi.transform.position;
            dist = delta.magnitude;

            if (dist < crashRadius) // Are they in our radius
                crashRisks.Add(b);
        }
        //Debug.Log("Crash Risks: " + crashRisks.Count);

        return (crashRisks);
    }


    public Vector3 GetAveragePosition(List<Boid> someBoids)
    {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids)
            if (b) // Skip if dead
                sum += b.transform.position;
        Vector3 center = sum / someBoids.Count;

        return (center);
    }


    public Vector3 GetAverageVelocity(List<Boid> someBoids)
    {
        Vector3 sum = Vector3.zero;
        foreach (Boid b in someBoids)
            if (b) // Skip if dead
                sum += b.velocity;
        Vector3 avg = sum / someBoids.Count;

        return (avg);
    }
}