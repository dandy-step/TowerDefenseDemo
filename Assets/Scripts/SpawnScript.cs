using UnityEngine;

public class SpawnScript : MonoBehaviour {
    public GameObject enemy;
    public GameObject boss;
    public GameLogic gameLogic;
    public float spawnTimeLimit;
    public float timer = 0f;
    public int wakeAtWaveNum;
    public GameObject[] waypointList;
    public float spawnRandomizerValue;      //used to make spawning seem less robotic
    public bool awake;
    public bool haveToSpawnBoss;

    public void SpawnBoss() {
        GameObject boss_ = Instantiate(boss, transform.position, transform.rotation, transform);
        boss_.GetComponent<EnemyClass>().SetHealth(gameLogic.bossHealth);
        boss_.transform.localScale += new Vector3(2f, 2f, 2f);        //make it big and mean
    }
	
	void Update () {
        //wake up spawn once it hits the wave manually set on the object
        if (!awake) {
            if (gameLogic.currentWave >= wakeAtWaveNum) {
                awake = true;
            } else {
                awake = false;
            }
        }

        if (awake) {
            timer += Time.deltaTime;
            if (timer >= (spawnTimeLimit + spawnRandomizerValue) && gameLogic.enemiesLeft > 0) {
                timer = 0f;
                gameLogic.enemiesLeft--;
                spawnRandomizerValue = Random.Range(-0.75f, 0.75f);

                if (gameLogic.enemiesLeft > 0) {
                    Instantiate(enemy, transform.position, transform.rotation, transform);
                }
            }
        }

        if (haveToSpawnBoss) {
            if (timer >= (spawnTimeLimit + 4f)) {       //give some extra time to spawn the boss so he doesn't clip through an enemy
                SpawnBoss();
                haveToSpawnBoss = false;
            }
        }
	}
}
