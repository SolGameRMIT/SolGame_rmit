using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidField : MonoBehaviour
{
    private float slowingEffect = 0.9f;

    private void OnTriggerStay(Collider collision)
    {
        PlayerController ship = collision.GetComponent<PlayerController>();
        if (ship != null)
        {
            if (!ship.Invincibility)
            {
                ship.ChangeHealth(-1);
            }
        }
        RaceA_FSMController controllerA = collision.GetComponent<RaceA_FSMController>();
        if(controllerA != null)
        {
            controllerA.GetComponent<Rigidbody>().velocity = controllerA.GetComponent<Rigidbody>().velocity * slowingEffect;
        }
        RaceB_FSMController controllerB = collision.GetComponent<RaceB_FSMController>();
        if (controllerB != null)
        {
            controllerB.GetComponent<Rigidbody>().velocity = controllerB.GetComponent<Rigidbody>().velocity * slowingEffect;
        }
    }
}
