using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] GameObject droid;
    private DroidStatus droidStatus;
    // Use this for initialization
    void Start()
    {
        droidStatus = droid.GetComponent<DroidStatus>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Race_C")
        {
            //print("hit coin");

            //droidStatus.CollectCoin(this.gameObject); // Using "this" is actually unnecessary here, I just think it improves readability
        }
        else if (other.gameObject.name == "Bolt Bullet")
        {
            droidStatus.ShootTarget(this.gameObject);
            Destroy(other.gameObject);
        }
    }
}
