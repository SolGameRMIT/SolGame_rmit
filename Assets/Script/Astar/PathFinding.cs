using Assets.Script.Astar;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using System;


// Ref https://www.redblobgames.com/pathfinding/a-star/introduction.html
public class PathFinding : MonoBehaviour
{
    PathManager pathManager;
    worldGrid grid;
    private int NORMAL_STRAIGHT_COST = 10, NORMAL_DIAGONAl_COST = 14, OBSTACLE_STRAIGHT_COST = 20, OBSTACLE_DIAGONAL_COST = 28;
    public LayerMask unwalkableMask;
    private void Awake()
    {
        pathManager = GetComponent<PathManager>();
        grid = GetComponent<worldGrid>();
        unwalkableMask = LayerMask.GetMask("unwalkable");
    }

    public List<Vector3> findPath(Vector3 startPos, Vector3 targetPos, bool smootherPath)
    {

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        // UnityEngine.Debug.DrawLine(startNode.worldPostion, targetNode.worldPostion, Color.red);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fcost < currentNode.fcost || openSet[i].fcost == currentNode.fcost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode.worldPostion == targetNode.worldPostion)
            {
                return RetracePath(startNode, currentNode, smootherPath);
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        return null;
    }
    List<Vector3> RetracePath(Node startNode, Node endNode, bool smootherPath)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        List<Vector3> pathList = new List<Vector3>();
        foreach (Node node in path)
        {
            pathList.Add(node.worldPostion);
        }
        if (smootherPath)
        {
            try
            {
                return GetSmootherPath(path);
            }
            catch (Exception)
            {

                return null;
            }
        }




        else
            return pathList;
    }

    List<Vector3> SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                wayPoints.Add(path[i].worldPostion);
            }
            else
            {
                wayPoints[wayPoints.Count - 1] = path[i].worldPostion;
            }
            directionOld = directionNew;
        }
        return wayPoints;
    }

    public List<Vector3> GetSmootherPath(List<Node> path)
    {
        if (path != null)
        {
            //Make a Vec3 list with player vec3 in it
            List<Vector3> pathInVec3 = new List<Vector3>();
            foreach (Node n in path)
            {
                pathInVec3.Add(n.worldPostion);
            }

            //Draw debug lines for pathfinding before smoothing
            for (int i = 0; i < pathInVec3.Count - 1; i++)
            {
                UnityEngine.Debug.DrawLine(pathInVec3[i], pathInVec3[i + 1], Color.white);
            }

            //Path smoothing
            //Create a new list for smoother path
            List<Vector3> pathSmoothing = new List<Vector3>();
            pathSmoothing.Add(pathInVec3[0]);

            //------
            int currIdx = 0;
            for (int i = 0; i < pathInVec3.Count - 1; i++)
            {
                if (i == currIdx)
                {
                    for (int j = i + 1; j < pathInVec3.Count; j++)
                    {

                        // Vector3 direction = (pathInVec3[j] - pathInVec3[i]).normalized * (pathInVec3[j] - pathInVec3[i]).magnitude;
                        // bool hit = Physics.Raycast(pathInVec3[i], direction, (pathInVec3[j] - pathInVec3[i]).magnitude, unwalkableMask);
                        // if (hit)
                        // {
                        //     pathSmoothing.Add(pathInVec3[j - 1]);
                        //     currIdx = j - 1;
                        //     break;
                        // }
                        // else
                        // {
                        //     for (float k = 0; k < (pathInVec3[j] - pathInVec3[i]).magnitude; k += 0.01f)
                        //     {
                        //         Vector3 P = Vector3.Lerp(pathInVec3[i], pathInVec3[j], k);
                        //         Vector3 n = new Vector3(Mathf.Round(P.x), P.y, Mathf.Round(P.z));
                        //         if (Physics.CheckSphere(n, grid.nodeRadius * 3f , unwalkableMask))
                        //         {
                        //             pathSmoothing.Add(pathInVec3[j - 1]);
                        //             currIdx = j - 1;
                        //             break;  
                        //         }
                        //     }
                        // }

                        //Improved version of pathfinding with different node cost. This only works with node nears obstacle has higher cost, other wise the smoothed path
                        //will not very different from the re-smoothed path.

                        Vector3 direction = (pathInVec3[j] - pathInVec3[i]).normalized * (pathInVec3[j] - pathInVec3[i]).magnitude;
                        {
                            for (float k = 0; k < (pathInVec3[j] - pathInVec3[i]).magnitude; k += 0.01f)
                            {
                                Vector3 P = Vector3.Lerp(pathInVec3[i], pathInVec3[j], k);
                                Vector3 n = new Vector3(Mathf.Round(P.x), P.y, Mathf.Round(P.z));
                                List<Node> nodeList = grid.GetNeighbours(grid.NodeFromWorldPoint(n));
                                bool existed = false;
                                foreach (Node node in nodeList)
                                {
                                    if (node.costMultiplier != Node.initCostMultiplier)
                                    {
                                        existed = true;
                                        break;
                                    }
                                }
                                if (existed)
                                {
                                    pathSmoothing.Add(pathInVec3[j - 1]);
                                    currIdx = j - 1;
                                    break;
                                }
                            }
                        }



                    }
                }
            }
            pathSmoothing.Add(pathInVec3[pathInVec3.Count - 1]);
            //------
            //Draw debug lines for path after smoothing
            for (int i = 0; i < pathSmoothing.Count - 1; i++)
            {
                UnityEngine.Debug.DrawLine(pathSmoothing[i], pathSmoothing[i + 1], Color.cyan);
            }
            return pathSmoothing;
        }
        return null;
    }


    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int maxCostMultiplier = 0;
        foreach (Node n in grid.GetNeighbours(nodeA))
        {
            if (n.costMultiplier > maxCostMultiplier)
            {
                maxCostMultiplier = n.costMultiplier;
            }
        }
        foreach (Node n in grid.GetNeighbours(nodeB))
        {
            if (n.costMultiplier > maxCostMultiplier)
            {
                maxCostMultiplier = n.costMultiplier;
            }
        }
        return NORMAL_DIAGONAl_COST * maxCostMultiplier * Mathf.Min(dstX, dstY) + NORMAL_STRAIGHT_COST * maxCostMultiplier * Mathf.Abs(dstX - dstY);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //https://docs.unity3d.com/ScriptReference/Physics.RaycastAll.html
        // Creat a ray to Find the closest point to seeker
        /*
        var ray = new Ray(seeker.position, target.position - seeker.position);
        var result = Physics.RaycastAll(ray, float.MaxValue);
        foreach (RaycastHit rayCastHit in result)
        {
            if(rayCastHit.transform == target)
            {
                var HitPoint = rayCastHit.point;
                findPath(seeker.position, HitPoint);
            }
        }
        */
    }
    private void FixedUpdate()
    {

    }
}
