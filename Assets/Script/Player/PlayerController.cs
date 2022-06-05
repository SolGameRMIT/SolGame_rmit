using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : Agent
{

    // Store a reference to the Rigidbody2D component required to use 2D Physics.
    private Rigidbody2D rb2d;
    private SpriteRenderer sr;

    private Vector3 birdStartPos;
    Rigidbody m_Rigidbody;
    public Spanwer m_Spanwer;
    private float nextFile;
    public float fileRate;

    GameObject shot;
    int weapon;
    public GameObject weaponType1;
    public GameObject weaponType2;
    public GameObject weaponType3;
    public Transform shotSpawn;
    public Boundary boundary;

    private float mouseAngle;

    // Reinforcement Learning variables
    private GameObject[] targets;
    private GameObject closestTarget;
    //private GameObject secondTarget;
    public int SpawnRate;
    private int powerupsIced = 0;
    public int powerupsToIce;
    private Vector3 startPos;
    public GameObject upgrade;


    //Death explosion
    public GameObject deathExplosion;

    //Health and invincibility related variables
    public float InvincibilityTimer = 2.0f; // The amount of  time the ship is invincible after a hit
    public int Maximumhealth = 5; //The maximum health of the ship
    int currentHealth; //The current health of the ship
    float InvincibleTime; //The remaining time of the ship's invincibility
    bool Invincible; //If the ship is invincible or not (to prevent insta-death)

    //Read only variables accessable from outside the script
    public int Health { get { return currentHealth; } }
    public bool Invincibility { get { return Invincible;  } }
    public int Weapon { get { return weapon; } }


    //Thrust and movement related variables
    float thrust = 0.001f; // How long we have to hold to get to max speed (higher quicker)
    float maxThrust = 0.01f; // Max speed
    float spaceDrag = 0.95f; // How we slow down naturally
    float acceleratorCooloff = 0.93f; // How our accelerator cools off
    float rotationSpeed = 0.1f; // How quick we turn

    // Internal calc vars
    float angle = 0.0f;
    float acceleration;
    Vector3 velocity;

    

    //audio sources
    public AudioClip bolt;
    public AudioClip blast;
    public AudioClip swirl;
    public AudioClip damage;
    public AudioClip increaseHealth;
    public AudioClip explosion;

    AudioSource Bolt;
    AudioSource Blast;
    AudioSource Swirl;
    AudioSource Damage;
    AudioSource IncreaseHealth;
    AudioSource Explosion;

    // Start is called before the first frame update
    void Start()
    {
        // enable external MLagents
        Application.runInBackground = true;
        MaxStep = 2000;

        //Fetch the Rigidbody component you attach from your GameObject
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();

        // Reinforcement Learning
        Debug.Log("Starting location: " + transform.position);
        startPos = transform.position;
        boundary.xMin += startPos.x;
        boundary.xMax += startPos.x;
        boundary.zMin += startPos.z;
        boundary.zMax += startPos.z;


        //set the initial health of the ship
        currentHealth = Maximumhealth;

        //default weapon setup
        shot = weaponType1;
        weapon = 1;
    }

    public override void OnEpisodeBegin()
    {
        
        Debug.Log("WE BEGINNING!!!");
        //Debug.Log(startPos);
        
        // Agent setup
        m_Rigidbody.MovePosition(startPos);
        gameObject.transform.position = startPos;
        if (m_Spanwer != null)
            m_Spanwer.reBoid();
        m_Rigidbody.velocity = Vector3.zero;

        // destroy all targets
        DestroyAllObjects();

        // set up targets 
        //MonoSub.Instantiate(upgrade, new Vector3(startPos.x + Random.Range(-4.0f,+4.0f), startPos.y, (startPos.z + Random.Range(-4.0f, +4.0f))), gameObject.transform.rotation);
        
        // Initial Training
        //MonoSub.Instantiate(upgrade, new Vector3(startPos.x, startPos.y, (startPos.z + 5.0f)), gameObject.transform.rotation);

        // Spawn targets around the field
        //spawnTargets();

        // Find the targets and place them into an array
        //this.targets = GameObject.FindGameObjectsWithTag("target");

        // Find the closest target and set this as the goal
        //FindClosestTarget(this.targets);
    }

    public void FindClosestTarget(GameObject[] targets)
    {
        if (targets.Length > 0)
        {
            GameObject closest = null;
            float distance = 1000000;
            float distanceTo = 0;
            foreach (GameObject target in targets)
            {
                distance = Vector3.Distance(m_Rigidbody.transform.position, target.transform.position); 
                if(distanceTo < distance)
                {
                    closest = target;
                    distanceTo = distance;
                    Debug.Log("Found a close target");
                }
            }
            if (closest != null)
                this.closestTarget = closest;
            else
                Debug.Log("No target was found close to the agent");
        }
    }

    void DestroyAllObjects()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Target");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }

    public void SpawnTargets()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observe the velocity, and position of the agent
        if (m_Rigidbody != null)
        {
            sensor.AddObservation(m_Rigidbody.transform.position);
            sensor.AddObservation(velocity);
            //if(closestTarget != null)
                //sensor.AddObservation(Vector3.Distance(m_Rigidbody.transform.position, closestTarget.transform.position));
        }
    }


    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Variables
        // our descrete actions array
        int action = actionBuffers.DiscreteActions[0];
        float speed = 0.005f;
        float angleSpeed = 0.05f;
        float moveHorizontal = 0.0f;
        float moveVertical = 0.0f;
        
        // Discrete Actions (reverse, forward, left rotate, right rotate)
        switch (action)
        {
            case 1:
                moveHorizontal = 1.0f;
                break;
            case 2:
                moveVertical = -1.0f;
                break;
            case 3:
                moveVertical = 1.0f;
                break;
            case 4:
                break;
        }

        // Angle and accelation calculations
        angle += moveVertical*angleSpeed;

        if (acceleration > maxThrust)
            acceleration = maxThrust;

        // Physics calculation
        velocity.x += moveHorizontal * speed * Mathf.Cos(angle);
        velocity.y += moveHorizontal * speed * Mathf.Sin(angle);
        velocity *= spaceDrag;
        acceleration *= acceleratorCooloff;

        // Apply changes to the model
        m_Rigidbody.velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, maxThrust);
        m_Rigidbody.MovePosition(new Vector3(
            Mathf.Clamp(m_Rigidbody.position.x + velocity.x, boundary.xMin, boundary.xMax),
            0f,
            Mathf.Clamp(m_Rigidbody.position.z + velocity.y, boundary.zMin, boundary.zMax)
            ));
        m_Rigidbody.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg + 90, Vector3.down);

    }


    public void setBoidsSpawner(Spanwer spanwer)
    {
        this.m_Spanwer = spanwer;
    }

    void Update()
    {
        GetComponent<Rigidbody>().velocity = Vector3.ClampMagnitude(GetComponent<Rigidbody>().velocity, maxThrust);
    }

    // Health related functions
    public void ChangeHealth(int amount)
    {
        if(amount < 0)
        {
            
            if (Invincible)
            {
                return;
            }
            else
            {
                //Debug.Log("Start invincibility");

                // Start invincibility
                Invincible = true;
                InvincibleTime = InvincibilityTimer;
                Damage.Play();
                var dieAnimationObject = MonoSub.Instantiate(deathExplosion, gameObject.transform.position, gameObject.transform.rotation);
                AudioSource.PlayClipAtPoint(explosion, this.gameObject.transform.position);
            }
        }
        else
        {
            IncreaseHealth.Play();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, Maximumhealth);
        //Debug.Log("Ship Health: " + currentHealth);
        
    }

    //Change weapon functions
    public void ChangeWeapon(int weaponSwap)
    {
        if (weaponSwap == 1)
        {
            shot = weaponType1;
            weapon = 1;
        }
        else if (weaponSwap == 2)
        {
            shot = weaponType2;
            weapon = 2;
        }
        else
        {
            shot = weaponType3;
            weapon = 3;
        }
    }

    //spawn explosion
    public void killShip()
    {
        var dieAnimationObject = MonoSub.Instantiate(deathExplosion, gameObject.transform.position, gameObject.transform.rotation);
        AudioSource.PlayClipAtPoint(explosion, this.gameObject.transform.position);
    }

    // Mouse firing related functions
    void getMouseAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.z = 0;
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        this.mouseAngle = Mathf.Atan2(mousePos.x, mousePos.y) * Mathf.Rad2Deg;
    }

    //playing audio functions
    void playShot()
    {
        if(weapon == 1)
        {
            Blast.Play();
        }
        else if (weapon == 2 )
        {
            Bolt.Play();
        }
        else
        {
            Swirl.Play();
        }
    }

    public AudioSource AddAudio(bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.name.Contains("Boid"))
        {
            Destroy(other.gameObject);
            AddReward(3.0f);
            powerupsIced++;
            Debug.Log("ICED THAT TARGET BOII");     
            if(powerupsIced >= powerupsToIce)
            {
                Debug.Log("WE OUT OF TARGETS BOSS MAN");
                AddReward(6.0f);
                powerupsIced = 0;
                EndEpisode();
            }
            /**
            if(GameObject.FindGameObjectsWithTag("target").Length >= 0)
            {
                AddReward(6.0f);
                EndEpisode(); 
            }**/
        }
        else if(other.name.Contains("Border"))
        {
            AddReward(-3.0f);
            EndEpisode();
        }
    }


    private class MonoSub : MonoBehaviour
    {

    }
}
