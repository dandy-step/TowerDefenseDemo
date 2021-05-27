using UnityEngine;
using UnityEngine.SceneManagement;

public class Levels : MonoBehaviour {

	void Awake () {
        //sets up game logic variables for the kind of room which is loaded
        float difficultyModifier = 1.25f;           //how much to ramp up enemy count in between waves
        int initialWaveSize = 0;

        string currentLevel = SceneManager.GetActiveScene().name;
        GameLogic gameLogic = GetComponent<GameLogic>();

        if (currentLevel.Equals("RoomEasy")) {
            gameLogic.numWaves = 5;
            gameLogic.bossHealth = 10f;
            gameLogic.cannonPriceMultiplier = 1.1f;
            gameLogic.enemySpeed = 5f;
            gameLogic.enemySpeedMultiplier = 1.1f;
            difficultyModifier = 1.15f;
            initialWaveSize = 20;
        } else if (currentLevel.Equals("RoomHard")) {
            gameLogic.numWaves = 7;
            gameLogic.bossHealth = 15f;
            gameLogic.cannonPriceMultiplier = 1.15f;
            gameLogic.enemySpeed = 5f;
            gameLogic.enemySpeedMultiplier = 1.1f;
            difficultyModifier = 1.35f;
            initialWaveSize = 30;
        } else {
            Debug.Log("Unimplemented room!");
        }

        //generate wave sizes based on difficulty modifier and initial wave size
        gameLogic.waveSizes = new int[gameLogic.numWaves];
        gameLogic.waveSizes[0] = initialWaveSize;
        for (uint i = 1; i < gameLogic.numWaves; i++) {
            gameLogic.waveSizes[i] = Mathf.RoundToInt(gameLogic.waveSizes[i - 1] * difficultyModifier);
        }
    }
}
