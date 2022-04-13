using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Boid"))
        {
            Destroy(other.gameObject);
        }
    }
}
