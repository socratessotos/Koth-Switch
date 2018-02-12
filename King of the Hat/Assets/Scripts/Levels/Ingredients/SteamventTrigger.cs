using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamventTrigger : MonoBehaviour {

    public SteamventPlatforms[] connectedPlatforms;
    public SteamventTrigger[] connectedTriggers;

    public float reuseCooldown = 3f;
    float reuseTimer;
    bool useable = true;

    SpriteRenderer sprite;

    void Start() {
        sprite = GetComponent<SpriteRenderer>();
        reuseTimer = Time.time + reuseCooldown;
    }

    void Update() {
        
        if(!useable) {
            if (Time.time > reuseTimer) {
                useable = true;
                sprite.color = Color.white;
            }
        }

    }

    public void DisableTrigger() {
        useable = false;
        reuseTimer = Time.time + reuseCooldown;
        sprite.color = Color.red;
    }

    void OnTriggerEnter2D(Collider2D other) {

        if (!useable) return;

        if(other.transform.CompareTag("Hat")) {
            Hat hat = other.transform.GetComponent<Hat>();

            if(hat.isBeingThrown) {

                for(int i = 0; i < connectedPlatforms.Length; i++) {
                    connectedPlatforms[i].TriggerPlatform();
                }

                for(int i = 0; i < connectedTriggers.Length; i++) {
                    connectedTriggers[i].DisableTrigger();
                }
                
            }

        }
    }

}
