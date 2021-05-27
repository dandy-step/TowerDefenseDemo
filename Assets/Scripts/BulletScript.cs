using UnityEngine;

public class BulletScript : MonoBehaviour {
    public float bulletSpeed;
    float bulletKillPosition = 1000f;
	
	void Update () {
        //just keep going
        transform.Translate(0f, 0f, bulletSpeed * Time.deltaTime);
        if ((transform.position.x  > bulletKillPosition) || (transform.position.x < -bulletKillPosition) || (transform.position.y > bulletKillPosition) || (transform.position.y < -bulletKillPosition) || (transform.position.z > bulletKillPosition) || (transform.position.z < -bulletKillPosition)) {
            //kill it if it has gone too far instead of relying on a killbox
            Destroy(gameObject);
        }
	}

    private void OnTriggerEnter(Collider other) {
        //check that it hit an enemy
        if (other.tag.Equals("enemy")) {
            if (GetComponent<BoxCollider>().bounds.Intersects(other.bounds)) {
                //reduce enemy health, destroy bullet
                other.gameObject.GetComponent<EnemyClass>().enemyHealth -= 1f;
                Destroy(gameObject);
            }
        } else if (other.tag.Equals("terrain")) {
            Destroy(gameObject);
        }
    }
}
