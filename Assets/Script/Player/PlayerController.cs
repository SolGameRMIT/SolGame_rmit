using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    Rigidbody m_Rigidbody;

    private float nextFile;
    public float fileRate;

    GameObject shot;
    public GameObject weaponType1;
    public GameObject weaponType2;
    public GameObject weaponType3;
    public Transform shotSpawn;
    public Boundary boundary;

    private float mouseAngle;

    //Health and invincibility related variables
    public float InvincibilityTimer = 2.0f; // The amount of  time the ship is invincible after a hit
    public int Maximumhealth = 5; //The maximum health of the ship
    int currentHealth; //The current health of the ship
    float InvincibleTime; //The remaining time of the ship's invincibility
    bool Invincible; //If the ship is invincible or not (to prevent insta-death)

    //Read only variables accessable from outside the script
    public int Health { get { return currentHealth; } }

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

    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Rigidbody component you attach from your GameObject
        m_Rigidbody = GetComponent<Rigidbody>();

        //set the initial health of the ship
        currentHealth = Maximumhealth;

        shot = weaponType1;
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse inputs
        getMouseAngle();
        if (Input.GetButton("Fire1") && Time.time > nextFile)
        {
            nextFile = Time.time + fileRate;
            Debug.Log("father shotSpawn.transform.position " + shotSpawn.transform.position);
            GameObject temp = Instantiate(shot, shotSpawn.transform.position, Quaternion.Euler(0f, mouseAngle, transform.rotation.z));
            Destroy(temp, 10f);
        }

        //Keyboard inputs
        if (Input.GetKey(KeyCode.Space) && Time.time > nextFile)
        {
            nextFile = Time.time + fileRate;
            Debug.Log("father shotSpawn.transform.position " + shotSpawn.transform.position);
            GameObject temp = Instantiate(shot, shotSpawn.transform.position, Quaternion.AngleAxis(angle * Mathf.Rad2Deg - 90, Vector3.down));
            Destroy(temp, 5f);
        }

        //invincibility checks
        if (Invincible)
        {
            InvincibleTime -= Time.deltaTime;
            if(InvincibleTime < 0)
            {
                Debug.Log("End Invincibility");
                Invincible = false;
            }
        }
    }

    void FixedUpdate()
    {


        // Get our controller input and translate it to thrust / rotation           
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && acceleration < maxThrust)
            acceleration += thrust;

        if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && acceleration > -maxThrust)
            acceleration -= thrust;

        if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
            angle -= rotationSpeed;

        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
            angle += rotationSpeed;


        // Physics calculation
        velocity.x += acceleration * Mathf.Cos(angle);
        velocity.y += acceleration * Mathf.Sin(angle);
        velocity *= spaceDrag;
        acceleration *= acceleratorCooloff;


        GetComponent<Rigidbody>().position = new Vector3(
            Mathf.Clamp(m_Rigidbody.position.x + velocity.x, boundary.xMin, boundary.xMax),
            0f,
            Mathf.Clamp(m_Rigidbody.position.z + velocity.y, boundary.zMin, boundary.zMax)
            );
        GetComponent<Rigidbody>().rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg + 90, Vector3.down);

    }

    // Health related functions
    public void ChangeHealth(int amount)
    {
        if(amount < 0)
        {
            if(Invincible)
            {
                return;
            }
            else
            {
                Debug.Log("Start invincibility");

                // Start invincibility
                Invincible = true;
                InvincibleTime = InvincibilityTimer;
            }
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, Maximumhealth);
        Debug.Log("Ship Health: " + currentHealth);
    }

    //Change weapon functions
    public void ChangeWeapon(int weapon)
    {
        if (weapon == 1)
        {
            shot = weaponType1;
        }
        else if (weapon == 2)
        {
            shot = weaponType2;
        }
        else
        {
            shot = weaponType3;
        }
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
}