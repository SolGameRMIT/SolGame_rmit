using UnityEngine;

public class DroidStatus : MonoBehaviour
{
    private DroidAgent droidAgent;
    private bool shieldActivated = false;
    // Use this for initialization
    void Start()
    {
        droidAgent = GetComponent<DroidAgent>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void MissShot()
    {
        droidAgent.HandleMissShot();
    }
    public void ShootObstacle()
    {
        droidAgent.HandleShootObstacle();
    }

    public void HitBullet()
    {
        if (shieldActivated)
        {
            print("Shielded the bullet");
            droidAgent.HandleShieldSuccess();
        }
        else
        {
            print("Hit the bullet");
            droidAgent.HandleHitByPlayerBolt();
        }
    }
    public void HitObstacle()
    {
        droidAgent.HandleHitObstacle();
    }
    public void ShootTarget(GameObject coin)
    {
        droidAgent.HandleShootTarget();
    }

    public void ShieldActivate(bool status)
    {
        this.shieldActivated = status;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player Bolt")
        {
            HitBullet();
            Destroy(other.gameObject);
        }
    }
}
