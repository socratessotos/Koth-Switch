using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CastMember {

    public GameObject prefab;
    public Vector3 spawnPosition;

}

public class SpawnCast : MonoBehaviour {

    public CastMember[] cast;
    public bool castIsSpawned = false;

    public void SpawnTheCast() {

        if (castIsSpawned) return;
        
        for(int i = 0; i < cast.Length; i++) {

            if (cast[i] == null) continue;

            GameObject newCastMember = Instantiate(cast[i].prefab, cast[i].spawnPosition, Quaternion.identity) as GameObject;
            newCastMember.GetComponent<CharacterColorChanger>().Init(0, 0);
            newCastMember.layer = LayerMask.NameToLayer("Player 1");

        }

        castIsSpawned = true;

    }

}
