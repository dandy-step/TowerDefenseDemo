using UnityEngine;

public class EnemyClass : MonoBehaviour {
    public GameLogic gameLogic;
    public GameObject currentWaypoint;
    public uint currentWaypointIndex = 0;
    public float enemySpeed;
    public float enemyHealth;

    public void SetHealth(float health) {
        enemyHealth = health;
    }

    public virtual void Start() {
        gameLogic = GameObject.Find("GameLogic").GetComponent<GameLogic>();
        enemySpeed = gameLogic.enemySpeed;
        currentWaypoint = GetComponentInParent<SpawnScript>().waypointList[0];      //start at waypoint 0
        gameLogic.liveEnemies++;                                                    //increment number of live enemies
	}
	
	public virtual void Update () {
        //head towards next waypoint
        transform.SetPositionAndRotation(new Vector3(Mathf.MoveTowards(transform.position.x, currentWaypoint.transform.position.x, enemySpeed * Time.deltaTime), Mathf.MoveTowards(transform.position.y, currentWaypoint.transform.position.y, Time.deltaTime), Mathf.MoveTowards(transform.position.z, currentWaypoint.transform.position.z, enemySpeed * Time.deltaTime)), transform.rotation);

        //rotate towards goal
        Quaternion waypointRelativeRotation = Quaternion.LookRotation(currentWaypoint.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, waypointRelativeRotation, Time.deltaTime);

        //death condition
        if (enemyHealth <= 0f) {
            Destroy(gameObject);                            //kill
            gameLogic.liveEnemies--;                        //decrement number of live enemies
            gameLogic.score += 10;                          //add score
        }
    }

    private void OnTriggerEnter(Collider collision) {
        //logic for finding the next waypoint
        if (collision == (currentWaypoint.GetComponent<Collider>())) {
            currentWaypointIndex++;
            if (currentWaypointIndex < GetComponentInParent<SpawnScript>().waypointList.Length) {
                currentWaypoint = GetComponentInParent<SpawnScript>().waypointList[currentWaypointIndex];
            } else {
                //if we're out of the array, then head for the goal
                currentWaypoint = GameObject.Find("GameLogic").GetComponent<GameLogic>().endGoal;
            }
        }
    }
}
