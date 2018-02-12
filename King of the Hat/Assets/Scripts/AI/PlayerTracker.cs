using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour {

    private const int DIRECTIONS_COUNT = 16;
    private const int MAX_DISTANCE = 20;

    public LayerMask collisionMask;

    Dummy dummy;

    void Awake() {
        if (dummy == null)
            dummy = GetComponentInParent<Dummy>();
    }

    void Update() {
        
        if(!dummy.isDead) UpdateRaycastCircle();

    }

    void UpdateRaycastCircle() {
        float angle = 0;

        for(int i = 0; i < DIRECTIONS_COUNT; i++) {
            float x = Mathf.Sin(angle);
            float y = Mathf.Cos(angle);
            angle += 2 * Mathf.PI / DIRECTIONS_COUNT;

            Vector3 _dir = new Vector3(x, y, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.4f, _dir.normalized, MAX_DISTANCE, collisionMask);
            Debug.DrawLine(transform.position + Vector3.up * 0.4f, transform.position + _dir.normalized * MAX_DISTANCE, Color.blue);
            
            if(hit) {
                if (hit.transform.CompareTag("Solid")) continue;
                
                if (hit.transform.CompareTag("Player")) {          
                    Debug.DrawLine(transform.position + Vector3.up * 0.4f, _dir.normalized * MAX_DISTANCE, Color.yellow);

                    if(Time.time > dummy.throwTimer && dummy.isWearingHat)
                        dummy.hat.Throw(_dir.normalized, 0);

                }

                if (hit.transform.CompareTag("Hat")) {
                    Debug.DrawLine(transform.position, _dir.normalized * MAX_DISTANCE, Color.red);

                    dummy.CheckIfCloserTarget(hit.transform.gameObject);
                    dummy.SetStateToHunt();
                }

            }

        }

    }

}
