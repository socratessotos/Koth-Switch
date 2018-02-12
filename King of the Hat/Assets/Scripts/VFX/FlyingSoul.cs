using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingSoul : MonoBehaviour {

    Vector3 startPoint;
    Vector3 endPoint;

    Vector3 velocity;

    ParticleSystem ps;

    float flyTimer = 0f;
    float flyTime = 1f;

    void Start() {
        ps = GetComponentInChildren<ParticleSystem>();
    }

	public FlyingSoul(Vector3 _startPoint, Vector3 _endPoint, int _teamIndex) {

        startPoint = _startPoint;
        endPoint = _endPoint;

        SetColor(_teamIndex);
        
    }
	
	void Update () {

        flyTime += Time.deltaTime;

        transform.position = Vector3.Lerp(startPoint, endPoint, flyTimer / flyTime);

        if(flyTime >= flyTimer) {
            Destroy(gameObject);
        }

	}

    void SetColor(int _teamIndex) {

        var _main = ps.main;
        _main.startColor = PlayerColorManager.instance.players[_teamIndex - 1].hatFireColor;

    }

}