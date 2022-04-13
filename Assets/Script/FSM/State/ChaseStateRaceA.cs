using Assets.Script.Astar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChaseStateRaceA : IState
{
    RaceA_FSMController RaceA;
    private GameController gameController;
    GameObject player;

    GameObject m_GameObject;
    Rigidbody m_Rigidbody;
    FSM m_FSM;
    List<Vector3> path;
    Transform tf;
    float speed = 0.01f;
    private float nextFindPathTime;
    private float FindPathTimeRate = 1f;
    private float maxForce = 10f, maxSpeed = 10f, slowingRadius = 0.5f, drag = 2f;
    private float pathRadius = 0.001f, futureAhead = 0.25f, avoidanceDistance = 3f, avoidanceWidth = 2f;
    private int currentPath = 0;
    public ChaseStateRaceA(FSM fsm, GameObject _gameObject)
    {
        player = GameObject.Find("Player");
        m_GameObject = _gameObject;
        m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        m_Rigidbody.drag = drag;
        tf = m_GameObject.transform;

        m_FSM = fsm;
        var _gameController = GameObject.Find("GameController");
        if (_gameController != null)
        {
            gameController = _gameController.GetComponent<GameController>();
        }
    }

    public void setRaceA(RaceA_FSMController RaceA)
    {
        this.RaceA = RaceA;
    }


    public void OnEnter()   //  The method that should be performed to enter this state
    {


    }
    public void OnUpdate() //The method that should be executed to maintain this state
    {
        if (gameController == null)
        {
            return;
        }
        if (Time.time > nextFindPathTime)
        {

            nextFindPathTime = Time.time + FindPathTimeRate;
            try
            {
                TryGetPath(RaceA.GetComponent<Transform>().position);
            }

            catch (Exception e)
            {
                return;
            }

        }

        if (path != null)
        {
            speed = gameController.getDroidSpeed();
            FollowPath(path);
        }
    }
    public void OnExit() //The method that should be executed to exit this state
    {
        Debug.Log("I am ChaseState. OnExit()");
    }



    void TryGetPath(Vector3 end)
    {
        PathManager.Request(m_GameObject.transform.position, end, true, onPathFound);
    }

    void onPathFound(List<Vector3> _path, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            this.path = _path;
        }
    }
    bool PointIsOnPath(Vector3 start, Vector3 end, Vector3 point)
    {
        return Vector3.Dot((end - start).normalized, (point - end).normalized) < 0f && Vector3.Dot((start - end).normalized, (point - start).normalized) < 0f;
    }
    public static Vector3 Seek(Vector3 target, Vector3 position, Vector3 currentVelocity, float maxSpeed, float maxForce)
    {
        Vector3 desiredVelocity = (target - position).normalized * maxSpeed;
        Vector3 steering = Vector3.ClampMagnitude(desiredVelocity - currentVelocity, maxForce);
        return steering;
    }
    public static Vector3 Arrive(Vector3 target, Vector3 position, Vector3 currentVelocity, float maxSpeed, float maxForce, float slowingRadius)
    {
        Vector3 desiredVelocity = (target - position);
        float distance = desiredVelocity.magnitude;
        if (distance < slowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed * (distance / slowingRadius);
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }
        Vector3 steering = Vector3.ClampMagnitude(desiredVelocity - currentVelocity, maxForce);
        return steering;
    }

    void FollowPath(List<Vector3> pathList)
    {

        if (pathList != null && pathList.Count != 0)
        {
            float worldRecord = 10000000000f;
            Vector3 target = Vector3.zero;

            Vector3 futurePosition = m_Rigidbody.position + (m_Rigidbody.velocity.normalized * futureAhead);
            for (int i = 0; i < pathList.Count - 1; i++)
            {
                Vector3 start = pathList[i];
                Vector3 end = pathList[i + 1];
                Vector3 normalPoint = FindTarget(start, end, futurePosition);
                if (normalPoint.z < Mathf.Min(start.z, end.z) || normalPoint.z > Mathf.Max(start.z, end.z))
                {
                    normalPoint.z = end.z;
                }
                if (normalPoint.x < Mathf.Min(start.x, end.x) || normalPoint.x > Mathf.Max(start.x, end.x))
                {
                    normalPoint.x = end.x;
                }

                float distance = Vector3.Distance(futurePosition, normalPoint);

                if (distance < worldRecord)
                {
                    worldRecord = distance;
                    currentPath = i;
                    Vector3 dir = (end - start).normalized * futureAhead;
                    target = normalPoint + dir;
                }


            }
            if (worldRecord > pathRadius)
            {
                if (currentPath == pathList.Count - 2)
                {
                    var velocity = Arrive(target, m_Rigidbody.position, m_Rigidbody.velocity, maxSpeed, maxForce, slowingRadius);
                    m_Rigidbody.AddForce(velocity);
                    currentPath = 0;
                }
                else
                {
                    var velocity = Seek(target, m_Rigidbody.position, m_Rigidbody.velocity, maxSpeed, maxForce);
                    m_Rigidbody.AddForce(velocity);
                }
                //If want obstacle avoidance, uncomment this.
                // m_Rigidbody.AddForce(obstacleAvoidance());
            }
            Vector3 targetDir = target - m_Rigidbody.position;
            Quaternion toRotation = Quaternion.LookRotation(targetDir);
            tf.rotation = Quaternion.Slerp(tf.rotation, toRotation, 0.1f);    
        }
    }

    Vector3 FindTarget(Vector3 start, Vector3 end, Vector3 futurePostion)
    {
        Vector3 startToFuturePos = futurePostion - start;
        Vector3 pathDir = (end - start).normalized;
        startToFuturePos.y = 0f;
        pathDir.y = 0f;
        float dotProducts = Vector3.Dot(startToFuturePos, pathDir);
        pathDir *= dotProducts;
        pathDir += start;
        return pathDir;

    }


    Vector3 obstacleAvoidance()
    {
        float radius = 0.3f;
        Vector3 bottomRight = m_Rigidbody.position + (tf.right * radius) + (-tf.forward * radius);
        Vector3 bottomLeft = m_Rigidbody.position + (-tf.right * radius) + (-tf.forward * radius);
        Vector3 topRight = m_Rigidbody.position + ((tf.right * radius * avoidanceWidth) + (tf.forward * avoidanceDistance));
        Vector3 topLeft = m_Rigidbody.position + (-tf.right * radius * avoidanceWidth) + (tf.forward * avoidanceDistance);
        // Debug.DrawRay(bottomRight, topRight - bottomRight, Color.green);
        // Debug.DrawRay(bottomLeft, topLeft - bottomLeft, Color.green);
        // Debug.DrawRay(bottomRight, bottomLeft - bottomRight, Color.green);
        // Debug.DrawRay(topRight, topLeft - topRight, Color.green);
        RaycastHit[] hits = new RaycastHit[2];
        bool[] isHit = new bool[2];
        isHit[0] = Physics.Raycast(bottomLeft, topLeft - bottomLeft, out hits[0], avoidanceDistance, LayerMask.GetMask("unwalkable"));
        isHit[1] = Physics.Raycast(bottomRight, topRight - bottomRight, out hits[1], avoidanceDistance, LayerMask.GetMask("unwalkable"));
        // Debug.DrawLine(m_Rigidbody.position, hits[0].point, Color.green);
        // Debug.DrawLine(m_Rigidbody.position, hits[1].point, Color.red);
        int leftOrRight = isHit[0] ? 0 : isHit[1] ? 1 : -1;
        if (leftOrRight != -1)
        {
            Vector3 dir = leftOrRight == 0 ? topRight : topLeft;
            Vector3 steeringToAvoid = dir + m_Rigidbody.position - hits[leftOrRight].collider.transform.position;
            steeringToAvoid *= Vector3.Distance(m_Rigidbody.position, hits[leftOrRight].collider.transform.position);
            return Seek(steeringToAvoid, m_Rigidbody.position, m_Rigidbody.velocity, maxSpeed, maxForce);
        }
        else
        {
            return Vector3.zero;
        }

    }
}


