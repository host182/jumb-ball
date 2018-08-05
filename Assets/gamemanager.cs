using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemanager : MonoBehaviour {

	public GameObject spike_l;
	public GameObject spike_r;
	public Vector3 spawnValues;
	public float wait_low, wait_high;

	public float speed;


	public float scroll_speed;


	void Start () {
		StartCoroutine (SpawnSpikes());
	}
	

	IEnumerator SpawnSpikes () {
        while (true) {
			yield return new WaitForSeconds (Random.Range(wait_low, wait_high));   //wait time

			//spike left-side
			if (Random.Range(0.0f,1.0f) > 0.5){
				Vector3 spawnPosition = new Vector3 (spawnValues.x, spawnValues.y, spawnValues.z);
				Instantiate (spike_r, spawnPosition, Quaternion.Euler(0,0,180));
			}
			//spike on right side
			else {
				Vector3 spawnPosition = new Vector3 (-spawnValues.x, spawnValues.y, spawnValues.z);
				Instantiate (spike_l, spawnPosition, transform.rotation);
			}
        }
    }

}

