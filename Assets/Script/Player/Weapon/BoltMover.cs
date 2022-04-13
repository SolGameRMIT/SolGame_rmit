using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltMover : MonoBehaviour
{
    public GameObject hited;
    public float speed;

    Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnDestroy()
    {
        var temp = Instantiate(hited, transform.position, Quaternion.Euler(90f, 90f, 0f));
        Destroy(temp, 0.4f);
    }

}
