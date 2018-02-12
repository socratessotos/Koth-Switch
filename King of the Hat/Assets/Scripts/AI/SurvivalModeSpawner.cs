using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivalModeSpawner : MonoBehaviour {

    public GameObject zombiePrefab;
    public GameObject zombieSpawnEffectPrefab;

    Level currentLevel;

    float spawnTimer;
    float spawnTime = 5;

    public List<GameObject> zombies = new List<GameObject>();
    int maxSpawn = 10;

    SmashCam smashCam;

    void Start() {
        currentLevel = GameController.instance.game.level;
        smashCam = GameController.instance.smashCam;

        spawnTimer = Time.time + spawnTime;
        SpawnZombie();

    }

    void Update() {

        if (GameController.instance.game.state != Game.State.PLAYING) return;

        if(Time.time > spawnTimer) {
            CleanUpList();

            if (zombies.Count >= maxSpawn) return;

            SpawnZombie();
            ReduceSpawnTime();
        }

    }


    void SpawnZombie() {

        GameObject newDummy = Instantiate(zombieSpawnEffectPrefab, transform.position, Quaternion.identity) as GameObject;
        newDummy.transform.parent = GameController.instance.game.level.transform;

        int randomSpawn = Random.Range(0, 4);

        newDummy.transform.position = currentLevel.startingSpawnPoints[randomSpawn].position;

        smashCam.AddTarget(newDummy.transform);
        smashCam.CleanUp();

    }

    void ReduceSpawnTime() {

        if(spawnTime >= 1f) spawnTime -= 0.1f;

        spawnTimer = Time.time + spawnTime;

    }

    void CleanUpList() {
        
        for(int i = 0; i < zombies.Count - 1; i++) {

            if(zombies[i] == null) {
                zombies.RemoveAt(i);
            }

        }
        
    }

}
