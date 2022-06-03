using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField] private GameObject droid;
    private DroidStatus droidStatus;
    [SerializeField]
    private bool isBounder = false;
    // Use this for initialization
    void Start()
    {
        droidStatus = droid.GetComponent<DroidStatus>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Droid Bolt")
        {
            droidStatus.MissShot();
            Destroy(other.gameObject);
        }
        else if (other.name == "Race_C" && isBounder)
        {
            other.transform.localPosition = Vector3.zero;
        }
        else if (other.name == "Player Bolt")
        {
            Destroy(other.gameObject);
        }
    }

}
