using UnityEngine;
using UnityEngine.UI;

public class EndScript : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        //game over condition
        if (other.tag.Equals("enemy")) {
            GameLogic gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
            gameLogic.gameOver = true;          
            Destroy(other.gameObject);      //destroy enemy
            GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f);    //change color to red
        }
    }
}
