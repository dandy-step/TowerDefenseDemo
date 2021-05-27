using System.Collections.Generic;
using UnityEngine;

public class TowerShooting : MonoBehaviour {
    GameLogic gameLogic;
    Bounds[] wallBounds;
    public GameObject bullet;
    public GameObject plane;
    public GameObject shortest;             //selected target
    public bool towerPlaced;
    public bool validPlacement;
    public float shootDelay;
    public float timer;
    public float towerRotationSpeed;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    public bool pickedTarget;

    void Start () {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        GameObject[] roomTerrain = GameObject.FindGameObjectsWithTag("terrain");        //find all terrain
        wallBounds = new Bounds[roomTerrain.Length];
        for (uint i = 0; i < roomTerrain.Length; i++) {                                 //get bounds to check valid placement later
            wallBounds[i] = roomTerrain[i].GetComponent<Collider>().bounds;
        }
        towerPlaced = false;
	}

    void ShootAt(Transform trans) {
        GameObject bull = Instantiate(bullet, transform.position, Quaternion.Euler(0f, Vector3.Angle(trans.position, transform.position), 0f));
        bull.transform.rotation = plane.transform.rotation;     //align shot with where the plane was looking
    } 
	
	void Update () {
        timer += Time.deltaTime;
		if (!towerPlaced) {
            Vector3 worldCoords = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1.0f));
            worldCoords.y = 0f;
            transform.SetPositionAndRotation(worldCoords, transform.rotation);

            //check that it is a valid placement, and change material color to indicate it
            validPlacement = false;
            for (uint i = 0; i < wallBounds.Length; i++) {
                if (GetComponent<BoxCollider>().bounds.Intersects(wallBounds[i])) {
                    GetComponentInChildren<MeshRenderer>().material.color = new Color(0f, 1f, 0f);
                    validPlacement = true;
                    transform.position = new Vector3(transform.position.x, wallBounds[i].size.y, transform.position.z);
                    break;
                }
            }
            
            //change to red if invalid
            if (!validPlacement) {
                GetComponentInChildren<MeshRenderer>().material.color = new Color(1f, 0f, 0f);
            }

            //listen for click to confirm, escape to cancel
            if (Input.GetMouseButtonUp(0)) {
                if (validPlacement) {
                    //cannon placed
                    towerPlaced = true;
                    GetComponentInChildren<MeshRenderer>().material.color = new Color(1f, 1f, 0f);

                    //update game variables
                    gameLogic.score -= gameLogic.cannonPrice;
                    gameLogic.placingCannon = false;
                    gameLogic.cannonPrice = Mathf.RoundToInt(gameLogic.cannonPrice * gameLogic.cannonPriceMultiplier);
                }
            } else if (Input.GetKeyUp(KeyCode.Escape)) {
                Destroy(gameObject);
                gameLogic.placingCannon = false;
            }
        } else {
            //range detection handling
            if (enemiesInRange.Count > 0) {
                enemiesInRange.RemoveAll(GameObject => GameObject == null);     //clean list of dead or removed GameObjects
            }

            //pick a target from the list of enemies that have entered the collider
            if (enemiesInRange.Count > 0 && !pickedTarget) {
                shortest = enemiesInRange[0];

                for (int i = 0; i < enemiesInRange.Count; i++) {
                    if (Vector3.Distance(enemiesInRange[i].transform.position, transform.position) < Vector3.Distance(shortest.transform.position, transform.position)) {
                        shortest = enemiesInRange[i];
                    }
                }
                pickedTarget = true;
                timer = 0f;
            }

            if (pickedTarget && shortest == null) {
                //targeted enemy was destroyed, clear boolean so we search for a new one next frame
                pickedTarget = false;
            }

            //smoothly slerp rotation of plane towards the target
            if (shortest != null) {
                Quaternion goal = Quaternion.LookRotation(shortest.transform.position - plane.transform.position, Vector3.up);
                plane.transform.rotation = Quaternion.Slerp(plane.transform.rotation, goal, towerRotationSpeed * Time.deltaTime);
            }

            if (timer >= shootDelay && shortest != null) {
                ShootAt(shortest.transform);        //shoot at target
                timer = 0f;                        //reset shoot timer
                pickedTarget = false;               //clear picked target flag
                shortest = null;                    //clear target
            }
        }
	}

    private void OnTriggerEnter(Collider other) {
        //add enemies that enter collider to the list and set flag
        if (other.tag.Equals("enemy")) {
            if (other.GetComponent<Collider>().bounds.Intersects(GetComponent<SphereCollider>().bounds)) {
                enemiesInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        //remove them once they leave
        if (other.tag.Equals("enemy")) {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}
