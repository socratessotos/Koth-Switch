using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour {

    public GameObject prefab;
    public float spawnTime = 3;
    float spawnTimer;

    SurvivalModeSpawner spawner;
    SmashCam smashCam;

    void Start() {
        spawner = GameObject.Find("Survival_Spawner(Clone)").GetComponent<SurvivalModeSpawner>();
        smashCam = GameController.instance.smashCam;

        spawnTimer = Time.time + spawnTime;
    }

    void Update () {
		
        if(Time.time > spawnTimer) {
            SpawnPrefab();
            DestroyObject();
        }

	}

    void SpawnPrefab() {
        GameObject newPrefab = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
        newPrefab.transform.parent = GameController.instance.game.level.transform;

        spawner.zombies.Add(newPrefab);

        smashCam.AddTarget(newPrefab.transform);
        smashCam.CleanUp();

    }

    void DestroyObject() {
        Destroy(gameObject);
    }

}
