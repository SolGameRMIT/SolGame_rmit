using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject RaceBDroid;
    public GameObject RaceADroid;
    Spanwer spanwer;
    List<GameObject> RaceBDroidList = new List<GameObject>();
    List<GameObject> RaceADroidList = new List<GameObject>();
    GameObject ship;
    public PlayerController shipController;


    int litmitDroid = 5;
    int maxDroid = 1;
    float DroidSpeed = 0.01f;

    int level = 1;
    public Vector3 spawnValue;
    public int scoreToWin;

    int maxSpawnWaitTime = 5;

    public bool canSaveSmallShip = false;


    //end state variables
    private Vector3 endPos;
    private bool endState;
    public Text GameOverText;
    private bool isGameOver;
    public Text RestartText;
    private bool needRestart;

    //random enemy spawn
    private float barrierZTop = 5.20f;
    private float barrierZBottom = -8.1f;
    private float barrierXLeft = -7.4f;
    private float barrierXRight = 11.5f;

    // Start is called before the first frame update


    //-------------------UIsetting start --------------------

    int score = 0;
    public Text saveSmallShip;
    public Text SmallShip;
    public Text scoreText;
    public Text gameTime;
    public Text gameLevel;
    public Text healthText;
    public Text invincibilityText;
    public Text youLose;
    int hour;
    int minute;
    int second;
    float timeSpend = 0f;
    
    // health of ship information
    int health;

    //-------------------UIsetting end--------------------



    void Start()
    {
        GameOverText.text = "";
        isGameOver = false;
        needRestart = false;
        // open a new Thread
        StartCoroutine(spawnDroid());

        ship = GameObject.Find("Player");
        if (ship != null)
        {
            shipController = ship.GetComponent<PlayerController>();
        }
        spanwer = GetComponent<Spanwer>();
        shipController.setBoidsSpawner(spanwer);
        endPos = ship.transform.position;
        endState = false;
    }

    /// <summary>
    /// IEnumerator and startCoroutine just like a Thread.
    /// </summary>
    /// <returns></returns>
    IEnumerator spawnDroid()
    {
        var count = 0;
        while (true)
        {
            if (isGameOver)
            {
                yield return null;
            }
            // raceA spawn

            if (FindObjectsOfType<RaceA_FSMController>().Length < maxDroid)
            {
                for (int i = 0; i < maxDroid; i++)
                {
                    Vector3 spawnPosition = new Vector3(Random.Range(barrierXLeft, barrierXRight), 0, Random.Range(barrierZTop, barrierZBottom));
                    var raceA = Instantiate(RaceADroid, spawnPosition, Quaternion.identity);
                    raceA.name = "raceA " + count.ToString();
                    count++;
                    RaceADroidList.Add(raceA.gameObject);
                }
            }
            // raceB spawn
            if (FindObjectsOfType<RaceB_FSMController>().Length < maxDroid)
            {
                Vector3 spawnPosition = new Vector3(Random.Range(barrierXLeft, barrierXRight), 0, Random.Range(barrierZTop, barrierZBottom));
                var raceB = Instantiate(RaceBDroid, spawnPosition, Quaternion.identity);
                raceB.name = "raceB " + count.ToString();
                count++;
                RaceBDroidList.Add(raceB.gameObject);
            }



            yield return new WaitForSeconds(Random.Range(2, maxSpawnWaitTime));
        }


    }

    public RaceA_FSMController GetRaceA()
    {
        if (FindObjectsOfType<RaceA_FSMController>().Length > 0)
        {
            return FindObjectsOfType<RaceA_FSMController>()[0];
        }
        return null;
    }

    public void RaceADroid_Destory(GameObject RaceADroid)
    {
        RaceADroidList.Remove(RaceADroid);
    }
    public void RaceBDroid_Destory(GameObject RaceBDroid)
    {
        RaceBDroidList.Remove(RaceBDroid);
    }

    // Update is called once per frame
    void Update()
    {
        RestartCheck();
        if (isGameOver)
            return;
        saveSmallShipCheck();
        SmallShipCheck();
        gameTimeUpdate();
        gameLevelUpdate();
        LevelUpdate();
        HealthUpdate();
        InvincibilityUpdate();
        CheckEndState();
    }

    //end state checker
    void CheckEndState()
    {
        //check if ship is alive
        GameObject shipCheck = GameObject.Find("Player");

        if (shipCheck == null)
        {
            endState = true;
        }
        //if the ship is alive, update the end location with ship location
        else
        {
            endPos = shipCheck.transform.position;
        }

        if(endState)
        {
            youLose.gameObject.SetActive(true);
            endState = false;
            //Play sound?
            Invoke("resetGame", 5);
            Debug.Log("You lose!!!!");
            
        }
    }

    void saveSmallShipCheck()
    {
        if (canSaveSmallShip && !spanwer.isMoveForwarTarget)
        {
            saveSmallShip.text = "Press R to save the smaller ship";
        }
        else
        {
            saveSmallShip.text = "";
        }
    }
    void SmallShipCheck()
    {
        SmallShip.text = "Small Ship: "+ FindObjectsOfType<Boid_>().Length.ToString();
    }

    void resetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void updateScore()
    {
        if (isGameOver || scoreText == null)
            return;
        scoreText.text = string.Format("Score: {0}", score.ToString());
    }

    public void HealthUpdate()
    {
        if (isGameOver)
            return;
        if (GameObject.Find("Player") != null)
        {
            healthText.text = string.Format("Health: {0}", shipController.Health);
        }
    }
    public void InvincibilityUpdate()
    {
        if (isGameOver)
            return;
        if (GameObject.Find("Player") != null)
        {
            invincibilityText.text = string.Format("Invincible: {0}", shipController.Invincibility);
        }
    }

    public void RestartCheck()
    {
        if (needRestart)
        {
            foreach (SwirlMover item in FindObjectsOfType<SwirlMover>())
            {
                Destroy(item.gameObject);
            }
            foreach (BlastMover item in FindObjectsOfType<BlastMover>())
            {
                Destroy(item.gameObject);
            }
            foreach (BoltMover item in FindObjectsOfType<BoltMover>())
            {
                Destroy(item.gameObject);
            }
            



            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                SceneManager.LoadScene(0);
            }
        }
    }

    public void addScore(int value)
    {
        score += value;
        updateScore();
    }

    public void removeScore(int value)
    {
        score -= value;
        updateScore();
    }

    public void gameTimeUpdate()
    {
        if (isGameOver)
            return;
        timeSpend += Time.deltaTime;
        hour = (int)timeSpend / 3600;
        minute = ((int)timeSpend - hour * 3600) / 60;
        second = (int)timeSpend - hour * 3600 - minute * 60;
        gameTime.text = string.Format("Time:{0:D2}:{1:D2}", minute, second);
    }

    public void gameLevelUpdate()
    {
        if (isGameOver)
            return;
        gameLevel.text = string.Format("Level: {0}", level.ToString());
    }

    public void LevelUpdate()
    {
        if (isGameOver)
            return;
        if(score > scoreToWin)
        {
            canSaveSmallShip = true;

        }
        if (score > level * 10)
        {
            level++;
            if (maxDroid < litmitDroid)
            {
                maxDroid += 1;
            }

            DroidSpeed += DroidSpeed;
        }
    }

    public float getDroidSpeed()
    {
        return DroidSpeed;
    }
    public bool IsGameOver()
    {
        return isGameOver;
    }
    public void GameOver()
    {
        if (GameOverText == null || needRestart == null)
        {
            return;
        }
        isGameOver = true;
        needRestart = true;
        GameOverText.text = "Game Over";
        RestartText.text = "Press Space to Restart";
    }


}



