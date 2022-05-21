using UnityEngine;

public class DroidStatus : MonoBehaviour
{
    private int coinsCollected;
    private DroidAgent droidAgent;

    // Use this for initialization
    void Start()
    {
        droidAgent = GetComponent<DroidAgent>();
        coinsCollected = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShootObstacle(GameObject bomb)
    {
        bomb.SetActive(false);
        droidAgent.HandleShootObstacle();
    }
    public void CollectTarget(GameObject coin)
    {
        coinsCollected++;
        coin.SetActive(false);
        droidAgent.HandleCollectTarget();
    }

    public void HitObstacle(GameObject bomb)
    {
        bomb.SetActive(false);
        droidAgent.HandleHitObstacle();
    }
    public void ShootTarget(GameObject coin)
    {
        coinsCollected++;
        coin.SetActive(false);
        droidAgent.HandleShootTarget();
    }
}
