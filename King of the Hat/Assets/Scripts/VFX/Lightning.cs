using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour {

    public Transform origin;
    public Transform destination;

    Vector3 originVector;
    Vector3 destinationVector;

    public LineRenderer lineRenderer;

    public int minSegments = 4;
    public int maxSegments = 10;

    public float minVariance = 0.5f;
    public float maxVariance = 2f;

    public float onLifetime = 0.1f;
    public float offLifetime = 0.1f;
    public int minFlickers = 2;
    public int maxFlickers = 5;

    public Gradient[] colors;

    int numberOfPoints;
    Vector2 origin2Destination;
    float timer;
    Vector2 normal;
    int numberOfFlickers = 2;
    int currentFlicker = 0;
    float startWidth = 1f;
    float currentWidth;

    bool on = false;

    Vector3[] buffer;

    void Awake() {
        lineRenderer.sortingLayerName = "Front_VFX";
    }

    public void SetTargets(Transform _origin, /*Transform _destination,*/ int distance, bool fromHat, int _minSegments, int _maxSegments) {
        origin = _origin;
        //destination = _destination;

        if(fromHat) {
            originVector = _origin.position;
            destinationVector = _origin.position + (Vector3.up * distance);

            minFlickers = 2;
            maxFlickers = 2;
        } else {
            originVector = _origin.position + (Vector3.up * distance);
            destinationVector = _origin.position;

            minFlickers = 4;
            maxFlickers = 4;
        }

        minSegments = _minSegments;
        maxSegments = _maxSegments;

        Reset();
    }

    void Update() {
        //if (Input.GetKeyDown(KeyCode.X))
        //    Reset();

        if (on) {

            if (currentFlicker == 0)
                GrowLightning();
            else {
                if (Time.time >= timer + onLifetime) {
                    ToggleLightningOff();
                }
            }
            /*
            if (Time.time >= timer + onLifetime) {
                ToggleLightningOff();

                if (currentFlicker < numberOfFlickers)
                    currentFlicker++;

                if (currentFlicker == numberOfFlickers)
                    DestroyLightning();
            }
            */
        } else {

            if (Time.time >= timer + offLifetime && currentFlicker <= numberOfFlickers) {
                lineRenderer.colorGradient = colors[Random.Range(0, colors.Length)];
                lineRenderer.enabled = true;
                timer = Time.time;
                on = true;

            }

        }



    }

    /*void LateUpdate() {
        destination.position = new Vector3(destination.position.x, Mathf.Sin(Time.time) * 4, 0);
    }*/

    int offsetMod = 1;
    void DeterminePoints() {

        lineRenderer.positionCount = numberOfPoints;

        lineRenderer.SetPosition(0, origin.position);

        origin2Destination = destinationVector - originVector;

        Vector2 segment = origin2Destination / (numberOfPoints - 1);

        for (int i = 1; i < numberOfPoints - 1; i++) {

            float variance = Random.Range(minVariance, maxVariance);

            Vector2 p = (Vector2)originVector + segment * i;
            p += normal * Random.Range(minVariance, maxVariance) * offsetMod;
            p += origin2Destination.normalized * variance * offsetMod;

            lineRenderer.SetPosition(i, p);

            offsetMod = -offsetMod;
        }

        lineRenderer.SetPosition(numberOfPoints - 1, destinationVector);

    }


    void BufferPoints() {

        buffer = new Vector3[numberOfPoints];

        lineRenderer.positionCount = 1;
        if (currentFlicker == 0) {
            lineRenderer.SetPosition(0, originVector);
            buffer[0] = originVector;
        }

        origin2Destination = destinationVector - originVector;

        Vector2 segment = origin2Destination / (numberOfPoints - 1);

        for (int i = 1; i < numberOfPoints - 1; i++) {

            float variance = Random.Range(minVariance, maxVariance);

            Vector2 p = (Vector2)originVector + segment * i;
            p += normal * Random.Range(minVariance, maxVariance) * offsetMod;
            p += origin2Destination.normalized * variance * offsetMod;

            //lineRenderer.SetPosition(i, p);
            buffer[i] = p;

            offsetMod = -offsetMod;
        }

        //lineRenderer.SetPosition(numberOfPoints - 1, destination.position);
        buffer[numberOfPoints - 1] = destinationVector;

    }

    int growCounter = 0;
    int growIndex = 0;
    void GrowLightning() {

        if (growCounter++ % 3 == 0) {

            if (lineRenderer.positionCount < numberOfPoints) {
                lineRenderer.positionCount++;
                growIndex++;

                lineRenderer.SetPosition(growIndex, buffer[growIndex]);

            } else {

                ToggleLightningOff();

            }

        }

    }

    void ToggleLightningOff() {
        lineRenderer.enabled = false;
        timer = Time.time;
        on = false;
        growCounter = 0;
        growIndex = 0;

        numberOfPoints = Random.Range(minSegments, maxSegments);
        CalculateNormal();
        if (currentFlicker == 0)
            BufferPoints();
        else
            DeterminePoints();

        if (currentFlicker < numberOfFlickers)
            currentFlicker++;

        if (currentFlicker == numberOfFlickers)
            DestroyLightning();
    }

    void CalculateNormal() {

        normal = new Vector2(-(destinationVector.y - originVector.y), (destinationVector.x - originVector.x)).normalized;

    }

    void Reset() {
        timer = Time.time;
        numberOfFlickers = Random.Range(minFlickers, maxFlickers + 1);
        currentFlicker = 0;
        growCounter = 0;
        growIndex = 0;

        currentWidth = startWidth;

        lineRenderer.positionCount = 0;
        numberOfPoints = Random.Range(minSegments, maxSegments);

        CalculateNormal();
        BufferPoints();

        lineRenderer.colorGradient = colors[Random.Range(0, colors.Length)];

        lineRenderer.enabled = true;

        on = true;
    }

    /*void OnDrawGizmos() {
        Gizmos.color = Color.yellow;

        for(int i = 0; i < lineRenderer.numPositions; i++) {
            Gizmos.DrawCube(lineRenderer.GetPosition(i), Vector3.one * 0.2f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(lineRenderer.GetPosition(1), (Vector2) lineRenderer.GetPosition(1) + (normal * 3));

    }*/

    void DestroyLightning() {
        Destroy(gameObject);
    }
}