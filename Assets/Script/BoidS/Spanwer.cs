using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spanwer : MonoBehaviour
{
    public GameObject boidPref;
    public GameObject BoidsTargetObject;
    List<GameObject> boidsList = new List<GameObject>();
    [Range(1, 500)]
    public int startingCount = 250;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    // Start is called before the first frame update
    public bool isMoveForwarTarget = false;
    public float minSpawnerX = -15f;
    public float maxSpawnerX = 15f;
    public float minSpawnerZ = -15;
    public float maxSpawnerZ = 15;

    void Start()
    {
        for (int i = 0; i < startingCount; i++)
        {
            var count = 0;
            while (true)
            {
                count++;
                var postion = new Vector3(Random.Range(minSpawnerX, maxSpawnerX), 0, Random.Range(minSpawnerZ, maxSpawnerZ));
                if(count == 20)
                {
                    return;
                }
                if (isPositionEmpty(postion))
                {
                    var boid = Instantiate(boidPref,
                        postion,
                        Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)),
                        transform
                        );
                    boid.name = "Boid " + i;
                    boidsList.Add(boid);
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    bool isPositionEmpty(Vector3 position)
    {
        Collider[] contextColliders = Physics.OverlapSphere(position, neighborRadius);
        return contextColliders.Length == 0?true:false;
    }


}
