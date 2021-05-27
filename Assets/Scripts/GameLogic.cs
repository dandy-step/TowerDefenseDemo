using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameLogic : MonoBehaviour {
    public GameObject endGoal;
    public GameObject towerCannon;
    public int score;
    public uint currentWave;
    public int cannonPrice;
    public float cannonPriceMultiplier;
    public float enemySpeedMultiplier;
    public int numWaves;
    public int enemiesLeft;                                     //enemies left to spawn in this wave
    public bool allowedToSpawnCannon;
    public float nextWaveTimer;                                 //timer for pause in between waves
    public bool waveTimerActive;
    public int[] waveSizes;                                     //how many enemies each wave has
    public bool spawnedBoss;
    public float bossHealth;
    public int liveEnemies;                                     //how many enemies still alive
    public bool placingCannon;
    public float enemySpeed;
    public bool gameOver;

    //UI objects
    public Text scoreText;
    public Text gameOverText;
    public Text currentWaveText;
    public Text waveClearedText;
    public Text buyTowerButtonText;
    public Button buyTowerButton;
    public Button mainMenuGUIButton;
    public GameObject mainMenuGUIPanel;


	void Start () {
        //set up initial game state
        score = 50;                                             //free cannon at the start of the game
        cannonPrice = 50;
        allowedToSpawnCannon = false;
        gameOverText.enabled = false;                           //hide game over text
        buyTowerButton.onClick.AddListener(BuyTower);           //buy tower button click listener
        mainMenuGUIButton.onClick.AddListener(MainMenuGUI);     //main menu button click listener
        mainMenuGUIPanel.SetActive(false);                      //hide main menu but keep reference around
        waveClearedText.enabled = false;                        //hide wave cleared text

        currentWave = 0;
        enemiesLeft = waveSizes[0];
    }

    void ResetGame() {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Time.timeScale = 1;     //restore timer after picking a menu option
    }

    void StartGame() {
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1;
    }

    void EndGame() {
        Application.Quit();
        Time.timeScale = 1;
    }

    void BuyTower() {
        allowedToSpawnCannon = true;
    }

    void MainMenuGUI() {
        mainMenuGUIPanel.SetActive(!mainMenuGUIPanel.activeSelf);
        if (mainMenuGUIPanel.activeSelf) {
            //pause action if menu is open
            Time.timeScale = 0;
            Button[] mainMenuButtons = mainMenuGUIPanel.GetComponentsInChildren<Button>();
            for (uint i = 0; i < mainMenuButtons.Length; i++) {
                switch (mainMenuButtons[i].GetComponentInChildren<Text>().text) {
                    case "Reset Game":
                        mainMenuButtons[i].onClick.AddListener(ResetGame);
                        break;
                    case "Start Menu":
                        mainMenuButtons[i].onClick.AddListener(StartGame);
                        break;
                    case "End Game":
                        mainMenuButtons[i].onClick.AddListener(EndGame);
                        break;
                    default:
                        Debug.Log("ERROR: Unimplemented main menu button!");
                        break;
                }
            }
        } else {
            Time.timeScale = 1;
        }
    }

    void SpawnBoss() {
        //randomly pick a currently active spawn point to spawn a boss from
        GameObject[] spawnPointList = GameObject.FindGameObjectsWithTag("spawnpoint");
        GameObject[] activeSpawns = new GameObject[spawnPointList.Length];
        int activeSpawnCount = 0;
        for (uint i = 0; i < spawnPointList.Length; i++) {
            if (spawnPointList[i].GetComponent<SpawnScript>().awake) {
                activeSpawns[activeSpawnCount] = spawnPointList[i];
                activeSpawnCount++;
            }
        }

        Random.Range(0, activeSpawnCount);
        activeSpawns[Random.Range(0, activeSpawnCount)].GetComponent<SpawnScript>().haveToSpawnBoss = true;
    }
	
	void Update () {
        if (!gameOver) {
            //disable button if not enough points to buy a tower
            if (score >= cannonPrice && !placingCannon) {
                buyTowerButton.gameObject.SetActive(true);
            } else {
                buyTowerButton.gameObject.SetActive(false);
            }

            if (allowedToSpawnCannon) {
                Instantiate(towerCannon, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f)), transform.rotation, transform);
                allowedToSpawnCannon = false;
                placingCannon = true;
            }

            //timer until next wave starts
            if (waveTimerActive) {
                nextWaveTimer -= Time.deltaTime;

                //start next wave
                if (nextWaveTimer <= 0f) {
                    waveTimerActive = false;
                    waveClearedText.enabled = false;
                    spawnedBoss = false;
                    currentWave++;
                    enemiesLeft = waveSizes[currentWave];
                    enemySpeed *= enemySpeedMultiplier;         //increase enemy speed
                }
            }

            if (enemiesLeft <= 0) {
                //spawn boss at the end of the wave
                if (!spawnedBoss) {
                    SpawnBoss();
                    spawnedBoss = true;
                }

                if (liveEnemies <= 0 && !waveTimerActive) {
                    // victory condition
                    if (currentWave == numWaves - 1) {
                        currentWave++;
                        gameOverText.text = "Victory!";
                        gameOverText.enabled = true;
                        waveClearedText.enabled = false;
                        currentWaveText.enabled = false;
                        buyTowerButton.gameObject.SetActive(false);
                        MainMenuGUI();
                    }

                    //give player a breather and then advance to the next wave
                    if (currentWave < numWaves) {
                        GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("enemy");
                        if (remainingEnemies.Length <= 0) {
                            waveClearedText.text = "Wave " + (currentWave + 1) + " cleared!";
                            waveClearedText.enabled = true;
                            waveTimerActive = true;
                            nextWaveTimer = 5f;
                        }
                    }
                }
            }

            //update UI
            scoreText.text = "Cash: " + score.ToString();
            currentWaveText.text = "Wave: " + (currentWave + 1).ToString();
            buyTowerButtonText.text = "Buy\nTower\n$" + cannonPrice.ToString();
        } else {
            //game over
            gameOverText.enabled = true;
            currentWaveText.enabled = false;
            scoreText.enabled = false;
            buyTowerButton.gameObject.SetActive(false);
        }
    }
}
