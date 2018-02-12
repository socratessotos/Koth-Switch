using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningCart : MonoBehaviour {

    public Vector3 leftPosition;
    public Vector3 rightPosition;

    public bool isAtLeftPosition = true;

    public float hitPower = 40;
    public float stunTime = 0.025f;

    bool lerping = false;
    float lerpTimer = 0f;
    public float lerpSpeed = 4f;

    public bool isDangerous { get { return lerping; } }
    public bool isMovingLeft { get { return isAtLeftPosition; } }

    SpriteRenderer sprite;

    void Start () {
        sprite = GetComponent<SpriteRenderer>();

        transform.localPosition = (isAtLeftPosition) ? leftPosition : rightPosition;
        lerping = false;
        sprite.color = Color.white;
    }

    void Update () {

        if (hitFreeze) {

            if (freezeFrames < hitFreezeLength) {
                freezeFrames++;
            } else {
                freezeFrames = 0;
                hitFreeze = false;
            }

            return;
        }

        if (lerping) {

            lerpTimer += Time.deltaTime * lerpSpeed;

            if (isAtLeftPosition) {

                transform.localPosition = Vector3.Lerp(rightPosition, leftPosition, lerpTimer);

            } else {

                transform.localPosition = Vector3.Lerp(leftPosition, rightPosition, lerpTimer);

            }

            if (lerpTimer >= 1) {
                lerping = false;
                lerpTimer = 0;
                sprite.color = Color.white;
            }

        }

	}

    public void MoveCart(int _direction) {

        if (lerping) return;

        if (_direction == -1 && isAtLeftPosition) return;
        if (_direction == 1 && !isAtLeftPosition) return;

        isAtLeftPosition = !isAtLeftPosition;

        lerping = true;

        sprite.color = Color.red;
    }

    int freezeFrames = 0;
    float hitFreezeLength = 0;
    bool hitFreeze = false;

    public void ApplyFreezeFrames(int _numberOfFrames) {
        hitFreezeLength = _numberOfFrames;
        hitFreeze = true;
    }

    void OnTriggerEnter2D(Collider2D other) {
        
        if(other.transform.CompareTag("Player")) {
            
            if(lerping) {

                ApplyFreezeFrames(10);

                Player player = other.transform.GetComponent<Player>();
                int left = (isAtLeftPosition) ? -1 : 1;
                Vector3 dir = player.transform.position - transform.position;
                
                player.BlowBack(dir, hitPower, stunTime);
                
                
            }

        }

    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        float size = 0.3f;

        Vector3 loweredGizmoPosition = (Application.isPlaying) ? rightPosition : rightPosition + transform.position;
        Gizmos.DrawCube(loweredGizmoPosition, Vector3.one * size);

        Vector3 raiseGizmoPosition = (Application.isPlaying) ? leftPosition : leftPosition + transform.position;
        Gizmos.DrawCube(raiseGizmoPosition, Vector3.one * size);

    }

}
