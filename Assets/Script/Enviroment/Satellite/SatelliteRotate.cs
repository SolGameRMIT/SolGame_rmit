using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteRotate : MonoBehaviour
{
    public bool reverse;
    Vector3 rotate;
    float mod;
    float mod2;
    // Start is called before the first frame update
    void Start()
    {
        mod = 0f;
        mod2 = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(mod2, mod, 0);
        if (reverse)
        {
            mod -= 0.03f;
            mod2 -= 0.03f;
        }
        else
        {
            mod =+ 0.03f;
            mod2 =+ 0.03f;
        }
    }
}
