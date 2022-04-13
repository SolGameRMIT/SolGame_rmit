using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid_ : MonoBehaviour
{
    //To achieve full marks, the steering behaviours should be built primarily from scratch, 
    //i.e., not copied from a tutorial or another repo. 
    //(See Section 3 of the assignment spec for clarification.) 
    //In addition, all of the following must be satisfied: 
    //- At least one type of character uses boids-style flocking with separation, cohesion and alignment. Done
    //- Aesthetically, the flocking characters look like a real flock. The behaviour appears fluid, Done
    //i.e., individual flock members have slightly different velocities and don¡¯t stick to a rigid formation. Done
    //- Agents that do not use pathfinding have some sort of obstacle avoidance steering behaviour. Done
    //- Beyond flocking and obstacle avoidance, there are at least two more complex steering behaviours implemented, 
    //e.g.follow-the-leader, wall following, a seek behaviour that avoids overshooting (arrive), a seek behaviour that accounts for moving targets (offset pursuit).



    Spanwer spanwerController;
    Collider m_collider;
    List<Transform> NeighborsList = new List<Transform>();
    float randomVelocityFactor; //individual flock members have slightly different velocities and don¡¯t stick to a rigid formation
    [Range(0.1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(1f, 100f)]
    public float driveFactor = 10f;//Control the speed of object movement/velocity
    [Range(1f, 100f)]
    public float maxSpeed = 25f; //Maximum speed of object movement/velocity

    [Range(0f, 10f)]
    public float avoidanceRadiusMultiplier = 0.5f;
    public float SquareAvoidanceRadius { get { return neighborRadius * neighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier; } }

    Vector3 currentVelocity;


    public float AvoidanceWeights = 1;
    public float CohesionWeights = 1;
    public float AlignmentWeights = 1;
    public float ObstaclesWeights = 5;
    public float StayInRadiusWeight = 3;
    public float MoveForwarTargetWeights = 3;
    public float SmoothTime = 0.5f;
    
    static Vector3[] m_ObstanceRayDirection = null;
    Vector3 center;
    public float radius = 30f;

    void Awake()
    {
        var _gameController = GameObject.Find("GameController");
        if (_gameController != null)
        {
            spanwerController = _gameController.GetComponent<Spanwer>();
        }

        var t = gameObject.GetComponent<TrailRenderer>();
        t.startColor = RandomColor();
        t.endColor = RandomColor();

        setObstanceRayDirection();
        center = Vector3.zero;
    }

    void Start()
    {
        randomVelocityFactor = Random.Range(0.3f, 1f);
        this.m_collider = GetComponent<Collider>();
    }

    void Update()
    {
        //The velocity of the object needs to be recalculated with each update
        var tempvelocity = Vector3.zero;
        //Get neighbors based on radius
        GetNeighbors();
        tempvelocity += CalAlignment(); //alignment
        tempvelocity += CalAvoidance(); //separation
        tempvelocity += CalCohesion();  //cohesion
        tempvelocity += CalObstacles();//Agents that do not use pathfinding have some sort of obstacle avoidance steering behaviour.
        tempvelocity += CalMoveForwarTarget();//a seek behaviour that accounts for moving targets (offset pursuit)
        tempvelocity += CalStayInRadius();
        Move(tempvelocity);
    }

    Vector3 CalAlignment()
    {
        // if no neighbors, maintain current alignment
        if (NeighborsList.Count == 0)
        {
            return transform.forward;
        }
        //add all points together and average
        Vector3 alignmentMove = Vector3.zero;
        foreach (Transform item in NeighborsList)
        {
            alignmentMove += item.transform.forward;
        }
        alignmentMove /= NeighborsList.Count;
        alignmentMove = velocityHandling(alignmentMove, AlignmentWeights);
        return alignmentMove;
    }


    Vector3 CalAvoidance()
    {
        // if no neighbors, return no adjustment
        if (NeighborsList.Count == 0)
        {
            return Vector3.zero;
        }
        //add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int count = 0;
        foreach (Transform item in NeighborsList)
        {
            if (Vector3.SqrMagnitude(item.position - transform.position) < SquareAvoidanceRadius)
            {
                count++;
                avoidanceMove += transform.position - item.position;
            }

        }
        if (count > 0)
        {
            avoidanceMove /= count;
        }
        avoidanceMove = velocityHandling(avoidanceMove, AvoidanceWeights);
        return avoidanceMove;
    }

    Vector3 CalCohesion()
    {
        // if no neighbors, return no adjustment
        if (NeighborsList.Count == 0)
        {
            return Vector3.zero;
        }
        //add all points together and average
        Vector3 cohesionMove = Vector3.zero;
        foreach (Transform item in NeighborsList)
        {
            cohesionMove += item.position;
        }
        cohesionMove /= NeighborsList.Count;
        cohesionMove -= transform.position;
        cohesionMove = Vector3.SmoothDamp(transform.forward, cohesionMove, ref currentVelocity, SmoothTime);
        cohesionMove = velocityHandling(cohesionMove, CohesionWeights);
        return cohesionMove;
    }

    Vector3 CalObstacles()
    {
        Vector3 bestDir = transform.forward;
        int count = 0;
        float maxDis = 0;

        foreach (var dir in m_ObstanceRayDirection)
        {
            Vector3 tdir = transform.TransformDirection(dir);
            var ray = new Ray(transform.position, transform.TransformDirection(dir));
            var result = Physics.RaycastAll(ray, 1f);
            if (result.Length != 0)
            {
                foreach (RaycastHit raycastHit in result)
                {
                    if (raycastHit.transform.gameObject.name.Contains("Boid"))
                    {
                        continue;
                    }
                    if (raycastHit.transform.gameObject.name.Contains("Player"))
                    {
                        continue;
                    }
                    float dis = raycastHit.distance;
                    Debug.DrawRay(transform.position, tdir, Color.red, 1f);
                    if (dis > maxDis)
                    {
                        count++;
                        bestDir = tdir;
                        maxDis = dis;
                    }
                }
            }
            else
            {
                if (count == 0)
                {
                    return Vector3.zero;
                }
                else
                {
                    tdir = velocityHandling(tdir, ObstaclesWeights);
                    return tdir;
                }
            }
        }
        return Vector3.zero;
    }

    Vector3 CalMoveForwarTarget()
    {
        if (!spanwerController.isMoveForwarTarget || spanwerController.BoidsTargetObject == null)
        {
            return Vector3.zero;
        }
        //need to Move Forwar Target
        //transform.forward += ( targetDir - transform.forward ) * delta;
        var tempMove = spanwerController.BoidsTargetObject.transform.position - transform.position;
        tempMove = velocityHandling(tempMove, MoveForwarTargetWeights);
        return tempMove;
    }

    Vector3 CalStayInRadius()
    {
        Vector3 centerOffset = center - transform.position;
        float temp = centerOffset.magnitude / radius;
        if(temp < 0.9f)
        {
            return Vector3.zero;
        }
        centerOffset = centerOffset * temp * temp;
        centerOffset = velocityHandling(centerOffset, StayInRadiusWeight);
        return centerOffset;

    }


    Vector3 velocityHandling(Vector3 tempVelocity,float weights)
    {
        tempVelocity = tempVelocity * weights;
        if (tempVelocity != Vector3.zero)
        {
            if(tempVelocity.sqrMagnitude > weights * weights)
            {
                tempVelocity = tempVelocity.normalized;
                tempVelocity *= weights;
            }
        }
        return tempVelocity;
    }

    void Move(Vector3 velocity)
    {
        velocity *= driveFactor;
        if (velocity.sqrMagnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
        velocity *= randomVelocityFactor;
        velocity.y = 0;
        transform.forward = velocity;
        transform.position += velocity * Time.deltaTime;
    }


    void setObstanceRayDirection()
    {
        List<Vector3> dirs = new List<Vector3>();
        var numPoints = 180;
        var pow = 2.5f;
        var turnFraction = 0.618f;

        for (int i = 1; i < numPoints; ++i)
        {
            float dst = Mathf.Pow(i / (numPoints - 1f), pow);
            float angle = 2 * Mathf.PI * turnFraction * i;
            float x = dst * Mathf.Cos(angle);
            float y = dst * Mathf.Sin(angle);
            // only forward we need
            if (y > 0)
            {
                dirs.Add(new Vector3(x, 0, y).normalized);
            }
        }
        m_ObstanceRayDirection = dirs.ToArray();
    }

    void GetNeighbors()
    {
        NeighborsList.Clear();
        Collider[] contextColliders = Physics.OverlapSphere(transform.position, neighborRadius * 5);
        foreach (Collider c in contextColliders)
        {
            if (c == m_collider)
            {
                continue;
            }
            if (c.gameObject.name.Contains("Border"))
            {
                continue;
            }
            if (c.gameObject.name.Contains("BackGround") || c.gameObject.name == "BackGround")
                continue;
            NeighborsList.Add(c.transform);
        }
    }

    Color RandomColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);
        Color color = new Color(r, g, b);
        return color;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Race"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }


}
