using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private GameObject droid;
    private DroidStatus droidStatus;
    [SerializeField] private GameObject[] borders;

    // Use this for initialization
    void Start()
    {
        droidStatus = droid.GetComponent<DroidStatus>();
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
        GetComponent<Rigidbody>().inertiaTensorRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == borders[0] || other.gameObject == borders[1])
        {
            GetComponent<Rigidbody>().velocity = new Vector3(-GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);
        }
        else if (other.gameObject == borders[2] || other.gameObject == borders[3])
        {
            GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, -GetComponent<Rigidbody>().velocity.z);
        }
        if (other.gameObject.name == "Race_C")
        {
            droidStatus.HitObstacle();
            Destroy(this.gameObject);
        }
        else if (other.gameObject.name == "Droid Bolt")
        {
            droidStatus.ShootObstacle();
            Destroy(other.gameObject);
        }
    }
}
