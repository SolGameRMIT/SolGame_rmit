using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastMover : MonoBehaviour
{
    public GameObject hited;
    public float speed;

    Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        //Debug.Log(" transform.position " + transform.position);
        //Debug.Log(" transform.roation " + transform.rotation);
        //m_Rigidbody.velocity = new Vector3(transform.rotation.x, 0f, 0f) * speed;
        //var temp = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z);
        m_Rigidbody.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(" transform.position " + transform.position);
    }



    private void OnDestroy()
    {
        var temp = Instantiate(hited, transform.position, Quaternion.Euler(90f, 90f, 0f));
        Destroy(temp, 0.4f);
    }

}
