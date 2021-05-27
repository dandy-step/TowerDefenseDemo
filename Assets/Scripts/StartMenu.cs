using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {
    public Button easyButton;
    public Button hardButton;

    void EasyGame() {
        SceneManager.LoadSceneAsync("RoomEasy");
    }

    void HardGame() {
        SceneManager.LoadSceneAsync("RoomHard");
    }

    void Start () {
        easyButton.onClick.AddListener(EasyGame);
        hardButton.onClick.AddListener(HardGame);
	}
}
