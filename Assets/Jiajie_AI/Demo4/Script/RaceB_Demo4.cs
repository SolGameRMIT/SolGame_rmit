using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceB_Demo4 : MonoBehaviour
{

    GameController_Demo4 gameController;
    GameObject PlayerAgent;
    public GameObject Weapon;
    public float fileRate = 3f;
    float nextShootingTime = 0;
    List<GameObject> BulletsList = new List<GameObject>();
    int HP = 5;
    // Start is called before the first frame update

    void Start()
    {

    }
    public void Init(GameObject agent, GameController_Demo4 controller)
    {
        PlayerAgent = agent;
        gameController = controller;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 deltaVec = PlayerAgent.transform.position - transform.position;
        deltaVec.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(deltaVec);
        transform.rotation = rotation;
        Shoot();
    }

    public int getHp()
    {
        return this.HP;
    }

    void Shoot()
    {
        if (Time.time < nextShootingTime)
        {
            return;
        }
        nextShootingTime = Time.time + fileRate;

        GameObject temp = Instantiate(Weapon);
        temp.transform.localPosition = transform.localPosition;
        //temp.transform.localEulerAngles = Vector3.up * angle;
        temp.transform.localRotation = transform.localRotation;
        temp.GetComponent<Bullets_Demo4>().Init(PlayerAgent.GetComponent<PlayerAgent_Demo4>(),this);
        temp.transform.parent = transform.parent;
        BulletsList.Add(temp);
    }
    
    public void Hited()
    {
        this.HP -= 1;
        if (this.HP <= 0)
        {
            Death();
        }
    }
    public void Death()
    {
        PlayerAgent.GetComponent<PlayerAgent_Demo4>().AddReward(8f);
        this.gameObject.SetActive(false);
        this.HP = 5;
    }



    public void GameReset()
    {
        BulletsList.ForEach(item => Destroy(item));
        BulletsList.Clear();
        this.nextShootingTime = 0f;
        this.gameObject.SetActive(true);
        this.HP = 5;
    }
}
