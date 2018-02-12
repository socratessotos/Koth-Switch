using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour {


    public Vector3 minRotationVector;
    public Vector3 maxRotationVector;

    Vector3 rotationVector;

    float initialYPosition;

    void Start() {
        initialYPosition = transform.localPosition.y;

        float xRot = Random.Range(minRotationVector.x, maxRotationVector.x);
        float yRot = Random.Range(minRotationVector.y, maxRotationVector.y);
        float zRot = Random.Range(minRotationVector.z, maxRotationVector.z);

        rotationVector = new Vector3(xRot, yRot, zRot);
    }

    void Update() {
        transform.Rotate(rotationVector * Time.deltaTime);

        //transform.position = new Vector3(transform.position.x, transform.parent.position.y + initialYPosition + Mathf.Sin(Time.time * 10f) * 0.2f, transform.position.z);
    }

}