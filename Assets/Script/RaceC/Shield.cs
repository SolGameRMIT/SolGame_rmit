using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private DroidAgent droidAgent;

    // Start is called before the first frame update
    void Start()
    {
        droidAgent = GetComponentInParent<DroidAgent>();
    }
    void HitBullet()
    {
        print("Shielded the bullet");
        droidAgent.HandleShieldSuccess();
    }
    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player Bolt")
        {
            HitBullet();
            other.gameObject.SetActive(false);
            Destroy(other.gameObject);
        }
    }
}
