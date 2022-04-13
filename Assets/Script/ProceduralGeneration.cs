using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    /** TO DO
     *  put objects into a list and iterate the list instead of copying code
     * 
     */ 
    //current position tracking
    private Vector3 currPos;
    private Vector3 maxPos;

    //restriction positions outside the barriers
    private Vector3 stopAlgo;


    //Barriers
    public GameObject topFence;
    public GameObject bottomFence;
    public GameObject leftFence;
    public GameObject rightFence;

    private Vector3 leftPos;
    private Vector3 leftDimensions;

    private Vector3 topPos;
    private Vector3 topDimensions;

    private Vector3 bottomPos;
    private Vector3 rightPos;
    
    //Objects
    public GameObject planet1;
    public GameObject planet2;
    public GameObject planet3;
    public GameObject bigShip;
    public GameObject satelite;
    public GameObject asteroidField;
    public GameObject healthUpgrade;
    public GameObject majorHealthUpgrade;
    public GameObject weaponUpgrade1;
    public GameObject weaponUpgrade2;
    public GameObject weaponUpgrade3;
    public GameObject[] asteroids;
    private List<GameObject> planets = new List<GameObject>();
    private List<GameObject> healthUpgrades = new List<GameObject>();
    private List<GameObject> weaponUpgrades = new List<GameObject>();

    //Object scale references (can't get them off objects without more work)
    private float planetX = (24.08f - 21.94f);
    private float planetZ = (17.61f - 15.21f);
    private float bigShipX = (29.08f - 27.59f);
    private float bigShipZ = (28.19f - 22.34f);
    private float bigShipY = 6.25f; //1.1f
    private float asteroidFieldZ = (19.84f - 15.18f);
    private float asteroidFieldX = (35.08f - 30.91f);
    private float asteroidX = 4f;
    private float asteroidZ = 4f;
    private float sateliteX = (33.22f - 32.25f);
    private float sateliteZ = (18.00f - 16.91f);
    private float powerupX = (33.54f - 33.02f);
    private float powerupZ = (15.9f - 15.35f);


    private float emptyX = 2f;
    private float emptyZ = 1f;

    //RNG values
    private int emptySpace = 65;
    private int planetSpawn = 10;
    private int shipSpawn = 5;
    private int asteroidFieldSpawn = 5;
    private int asteroidSpawn = 20;
    private int sateliteSpawn = 5;
    private int powerupSpawn = 10;

    private int planetRNG;

    void Awake()
    {
        // add the RNG values
        planetRNG = emptySpace + planetSpawn;

        //Make the sure barriers are found
        if (leftFence == null)
        {
            leftFence = GameObject.Find("Border3");
        }

        if (bottomFence == null)
        {
            bottomFence = GameObject.Find("Border2");
        }

        if(topFence == null)
        {
            topFence = GameObject.Find("Border1");
        }

        if(rightFence == null)
        {
            rightFence = GameObject.Find("Border4");
        }

        //add the gameObjects to the list
        planets.Add(planet1);
        planets.Add(planet2);
        planets.Add(planet3);
        healthUpgrades.Add(healthUpgrade);
        healthUpgrades.Add(majorHealthUpgrade);
        weaponUpgrades.Add(weaponUpgrade1);
        weaponUpgrades.Add(weaponUpgrade2);
        weaponUpgrades.Add(weaponUpgrade3);

        //Call on awake so that other operations that rely on start can run successfully afterwards
        SpawnMap();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnMap()
    {
        //Find the dimensions of the walls so we can create our walls
        leftDimensions = leftFence.GetComponent<BoxCollider>().bounds.size;
        leftPos = leftFence.transform.position;
        //Debug.Log("Left Dimensions: " + leftDimensions);

        topDimensions = topFence.GetComponent<BoxCollider>().bounds.size;
        topPos = topFence.transform.position;

        rightPos = rightFence.transform.position;
        bottomPos = bottomFence.transform.position;

        // set the starting point of the algo
        currPos = leftPos;

        //make sure the position is away from the wall
        currPos.x = leftPos.x + leftDimensions.x * 1.1f; //* 2f;
        currPos.y = leftPos.y;
        currPos.z = topPos.z - topDimensions.z * 1.1f;

        

        //set up end condition for algo
        stopAlgo.x = rightPos.x;
        stopAlgo.z = bottomPos.z;

        bool algo = true;
        bool check = false;
        int rng = 0;
        //Start algo
        //int iterations = 0;
        while (algo)
        {
            //Debug.Log("starting position: " + currPos);
            check = false;
            rng = Random.Range(1, 100);
            //Empty space generate
            if (!check && rng <= emptySpace)
            {
                currPos.x += emptyX;
                if (emptyZ > maxPos.z)
                {
                    currPos.z -= planetZ * 0.5f;
                    maxPos.x = emptyZ;
                }
                check = true;
            }

            //spawn planet
            rng = Random.Range(1, 100);
            if (!check && rng <= planetSpawn)
            {
               // Debug.Log("position before Planet: " + currPos);
                currPos.x += planetX;
                //Debug.Log("CurrX : " + currPos.x + " + planetX: " + planetX);
                if (planetZ > maxPos.z)
                {
                    Debug.Log("PlanetZ bigger than maxPos");
                    currPos.z -= planetZ * 0.5f;
                    maxPos.z = planetZ;
                }
                if (currPos.z > stopAlgo.z && currPos.x * 1.1 <= stopAlgo.x)
                {
                    rng = Random.Range(0, planets.Count);
                    Instantiate(planets[rng], currPos, Quaternion.identity);
                }
                check = true;
                //Debug.Log("Spawned planet: " + currPos);
                
            }

            //spawn ship
            rng = Random.Range(1, 100);
           // Debug.Log("Rng before ship: " + rng);
            //Debug.Log("rng: " + rng + "ship: " + shipSpawn);
            if (!check && rng <= shipSpawn)
            {
                //Debug.Log("Position before ship: " + currPos);
                
                currPos.x += bigShipX;
                //Debug.Log("CurrX : " + currPos.x + " + shipX: " + bigShipX);
                if (bigShipZ > maxPos.z)
                {
                    //Debug.Log("ShipZ bigger than maxPos");
                    currPos.z -= bigShipZ * 0.5f;
                    maxPos.z = bigShipZ;
                }
                // Debug.Log("Spawned ship: " + currPos);
                if (currPos.z * 2 > stopAlgo.z && currPos.x * 1.1 <= stopAlgo.x)
                {
                    Instantiate(bigShip, currPos, Quaternion.identity);
                }
                check = true;
                
            }

            //spawn asteroid
            rng = Random.Range(1, 100);
            // Debug.Log("Rng before ship: " + rng);
            //Debug.Log("rng: " + rng + "asteroid: " + asteroidSpawn);
            if (!check && rng <= asteroidSpawn)
            {
                //Debug.Log("Position before ship: " + currPos);

                currPos.x += asteroidX;
                //Debug.Log("CurrX : " + currPos.x + " + shipX: " + bigShipX);
                if (asteroidZ > maxPos.z)
                {
                    //Debug.Log("ShipZ bigger than maxPos");
                    currPos.z -= asteroidZ * 0.5f;
                    maxPos.z = asteroidZ;
                }
                // Debug.Log("Spawned ship: " + currPos);
                if (currPos.z * 2 > stopAlgo.z && currPos.x * 1.1 <= stopAlgo.x)
                {
                    int rng2 = Random.Range(0, asteroids.Length);
                    print(asteroids[rng2]);
                    Instantiate(asteroids[rng2], currPos, Quaternion.identity);
                }
                check = true;

            }

            //spawn asteroid field
            rng = Random.Range(1, 100);
            // Debug.Log("Rng before ship: " + rng);
            //Debug.Log("rng: " + rng + "asteroid: " + asteroidFieldSpawn);
            if (!check && rng <= asteroidFieldSpawn)
            {
                //Debug.Log("Position before ship: " + currPos);

                currPos.x += asteroidFieldX;
                //Debug.Log("CurrX : " + currPos.x + " + shipX: " + bigShipX);
                if (asteroidFieldZ > maxPos.z)
                {
                    //Debug.Log("ShipZ bigger than maxPos");
                    currPos.z -= asteroidFieldZ * 0.5f;
                    maxPos.z = asteroidFieldZ;
                }
                // Debug.Log("Spawned ship: " + currPos);
                if (currPos.z > stopAlgo.z && currPos.x * 1.1 <= stopAlgo.x)
                {
                    Instantiate(asteroidField, currPos, Quaternion.identity);
                }
                check = true;

            }

            //satelite spawn
            rng = Random.Range(1, 100);
            // Debug.Log("Rng before ship: " + rng);
            //Debug.Log("rng: " + rng + "Satelite: " + sateliteSpawn);
            if (!check && rng <= sateliteSpawn)
            {
                //Debug.Log("Position before ship: " + currPos);

                currPos.x += sateliteX;
                //Debug.Log("CurrX : " + currPos.x + " + shipX: " + bigShipX);
                if (sateliteZ > maxPos.z)
                {
                    //Debug.Log("ShipZ bigger than maxPos");
                    currPos.z -= sateliteZ * 0.5f;
                    maxPos.z = sateliteZ;
                }
                // Debug.Log("Spawned ship: " + currPos);
                if (currPos.z > stopAlgo.z && currPos.x * 1.1 <= stopAlgo.x)
                {
                    Instantiate(satelite, currPos, Quaternion.identity);
                }
                check = true;
            }

            //power up spawn
            rng = Random.Range(1, 100);
            // Debug.Log("Rng before ship: " + rng);
            //Debug.Log("rng: " + rng + "Power up: " + powerupSpawn);
            if (!check && rng <= powerupSpawn)
            {
                //Debug.Log("Position before ship: " + currPos);

                currPos.x += powerupX;
                //Debug.Log("CurrX : " + currPos.x + " + shipX: " + bigShipX);
                if (powerupZ > maxPos.z)
                {
                    //Debug.Log("ShipZ bigger than maxPos");
                    currPos.z -= powerupZ * 0.5f;
                    maxPos.z = powerupZ;
                }
                // Debug.Log("Spawned ship: " + currPos);
                if (currPos.z > stopAlgo.z && currPos.x * 1.1 <= stopAlgo.x)
                {
                    rng = Random.Range(0, weaponUpgrades.Count);
                    Instantiate(weaponUpgrades[rng], currPos, Quaternion.identity);
                }
                check = true;
            }

            //health upgrades
            rng = Random.Range(1, 100);
            // Debug.Log("Rng before ship: " + rng);
            //Debug.Log("rng: " + rng + "Health Upgrades: " + powerupSpawn);
            if (!check && rng <= powerupSpawn)
            {
                //Debug.Log("Position before ship: " + currPos);
                currPos.x += powerupX;
                //Debug.Log("CurrX : " + currPos.x + " + shipX: " + bigShipX);
                if (powerupZ > maxPos.z)
                {
                    //Debug.Log("ShipZ bigger than maxPos");
                    currPos.z -= powerupZ * 0.5f;
                    maxPos.z = powerupZ;
                }
                // Debug.Log("Spawned ship: " + currPos);
                if (currPos.z > stopAlgo.z && currPos.x * 1.1 <= stopAlgo.x)
                {
                    rng = Random.Range(0, healthUpgrades.Count);
                    Instantiate(healthUpgrades[rng], currPos, Quaternion.identity);
                }
                check = true;
            }

            //if we have reached the wall
            if (currPos.x + maxPos.x >= stopAlgo.x)
            {
                //Debug.Log("We have hit a wall");
                //check if we have any space on the bottom
                if (currPos.z - maxPos.z * 1.1 < stopAlgo.z)
                {
                    //Debug.Log("we hit the bottom");
                    //if we don't have space, end the algo
                    algo = false;
                    break;
                }
                else
                { //if we do, continue to iterate downwards

                    //if we reach the wall, go back to starting x
                    currPos.z -= maxPos.z * 1.1f;
                }
                
                currPos.x = leftPos.x + leftDimensions.x * 1.1f;
                
            }
            //if we haven't, continue to iterate to the right
            else
            {
                currPos.x += maxPos.x;
            }
            check = false;
            /**iterations++;
            if (iterations > 200)
                algo = false;**/
        }
    }
}
