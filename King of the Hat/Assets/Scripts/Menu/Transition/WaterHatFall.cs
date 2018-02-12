using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHatFall : MonoBehaviour {

    public GameObject prefab;
    public Sprite[] hats;

    public float minSpeed = 1f;
    public float maxSpeed = 2f;

    public float minRotationSpeed = 50f;
    public float maxRotationSpeed = 200f;

    void Start() {
        SpawnHats();
    }

    void SpawnHats() {
        
    }

	void OnEnable() {
        RandomHatWaterfall(Vector2.right);
    }
	
    void RandomHatWaterfall(Vector2 _direction) {

        float xSpawn = _direction.x * -Screen.width;
        float ySpawn = Screen.height;

        for(int y = 0; y <= 80; y++) {
            for (int i = 0; i <= 10; i++) {
                GameObject newHat = Instantiate(prefab, Vector2.zero, Quaternion.identity) as GameObject;
                newHat.transform.SetParent(transform, false);

                float yVariance = Random.Range(-Screen.height / 20f, Screen.height / 20f);
                newHat.transform.localPosition = new Vector2(-Screen.width - y * Random.Range(0, 10), Screen.height - ((Screen.height * 2) / 10 * i) + yVariance);

                float newSpeed = Random.Range(minSpeed, maxSpeed);
                float newRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
                Sprite newSprite = hats[Random.Range(0, hats.Length - 1)];

                newHat.GetComponent<SingleHatWaterfall>().Init(_direction, newSpeed, newRotationSpeed, newSprite);

            }
        } 

    }

    void LateUpdate() {
        
        if(transform.childCount == 0) {
            gameObject.SetActive(false);
        }

    }

}
