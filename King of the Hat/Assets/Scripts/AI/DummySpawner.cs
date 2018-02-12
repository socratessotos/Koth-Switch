using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySpawner : MonoBehaviour {

    public GameObject dummyPrefab;

    public void SpawnDummy() {
        GameObject newDummy = Instantiate(dummyPrefab, transform.position, Quaternion.identity) as GameObject;
        newDummy.transform.parent = GameController.instance.game.level.transform;
        newDummy.transform.position = Vector3.zero;
        GameController.instance.smashCam.AddTarget(newDummy.transform);
   }

}
