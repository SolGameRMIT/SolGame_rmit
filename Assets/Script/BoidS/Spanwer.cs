using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spanwer : MonoBehaviour
{
    public GameObject boidPref;
    public GameObject BoidsTargetObject;
    public List<GameObject> boidsList = new List<GameObject>();
    [Range(1, 500)]
    public int startingCount = 250;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    // Start is called before the first frame update

    public GameObject player;
    public float modifier = 15f;
    public bool isMoveForwarTarget = false;
    public float minSpawnerX;
    public float maxSpawnerX;
    public float minSpawnerZ;
    public float maxSpawnerZ;

    private Vector3 startPos;

    void Start()
    {
        minSpawnerX = player.transform.position.x - modifier;
        maxSpawnerX = player.transform.position.x + modifier;
        minSpawnerZ = player.transform.position.z - modifier;
        maxSpawnerZ = player.transform.position.z + modifier;

        for (int i = 0; i < startingCount; i++)
        {
            var count = 0;
            while (true)
            {
                count++;
                var postion = new Vector3(Random.Range(minSpawnerX, maxSpawnerX), 0, Random.Range(minSpawnerZ, maxSpawnerZ));
                if (count == 40)
                    break;
                if (isPositionEmpty(postion))
                {
                    var boid = Instantiate(boidPref,
                        postion,
                        Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                        transform
                        );
                    boid.name = "Boid " + i;
                    boid.tag = "boid";
                    boidsList.Add(boid);
                    break;
                }
            }
        }
    }


    public void reBoid()
    {
        //this.startPos = startPos;
        foreach (GameObject boid in boidsList)
        {
            if (boid != null)
                Destroy(boid);
        }
        //boidsList.Clear();
        this.Start();
    }


    public int getBoidCount()
    {
        return boidsList.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isPositionEmpty(Vector3 position)
    {
        Collider[] contextColliders = Physics.OverlapSphere(position, neighborRadius);
        Debug.Log(contextColliders.Length);
        return contextColliders.Length == 0 ? true:false;
    }
}
