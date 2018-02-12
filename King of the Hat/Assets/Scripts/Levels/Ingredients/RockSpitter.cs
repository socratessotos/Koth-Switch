using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpitter : MonoBehaviour {

    public GameObject rockPrefab;

    public Vector2 launchDirection;

    public float spitSpeed = 10f;

    public float spawnTime = 3f;
    float spawnTimer;

	void Start () {
        spawnTimer = Time.time + spawnTime;
	}
	
	void Update () {
		
        if(Time.time > spawnTimer) {
            SpitRock();

            spawnTimer = Time.time + spawnTime;
        }

	}

    void SpitRock() {

        GameObject newRock = Instantiate(rockPrefab, transform.position, Quaternion.identity) as GameObject;

        newRock.GetComponent<TransferDanger>().velocity = launchDirection * spitSpeed;

    }

}
