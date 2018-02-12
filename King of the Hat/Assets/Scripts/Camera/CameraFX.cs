using UnityEngine;
using System.Collections;

[RequireComponent (typeof (AdvancedCA))]
public class CameraFX : MonoBehaviour {
		
	public static CameraFX instance;

		// Transform of the camera to shake. Grabs the gameObject's transform
		// if null.
	public Transform camTransform;
	public Camera cam;

		// How long the object should shake for.
	public float shakeDuration = 0f;

		// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	Vector3 originalPos;

	AdvancedCA ca;

	void Awake() {
			
		if (camTransform == null) {
				
			camTransform = GetComponent(typeof(Transform)) as Transform;
			cam = camTransform.GetComponent<Camera> ();
		}

		ca = GetComponent <AdvancedCA> ();

		if (instance == null) {

			instance = this;

		}

	}

	void OnEnable() {
			
		originalPos = camTransform.localPosition;

	}

	void Update() {
			
		if (shakeDuration > 0) {

			Vector2 shake = Random.insideUnitCircle * shakeAmount;

			camTransform.localPosition += new Vector3(shake.x, shake.y, 0);


			shakeDuration -= Time.deltaTime * decreaseFactor;

		} else {
				
			shakeDuration = 0f;

		}

	}

	public void ShakeCamera (float duration, float amplitude, float decay) {

		shakeDuration = duration;
		shakeAmount = amplitude;
		decreaseFactor = decay;

	}

	public IEnumerator HitPlayerAnimation () {

		ShakeCamera (0.3f, 0.3f, 10f);


		float t = 0;

		while (t < 1) {

			t += Time.deltaTime * 25f;

			ca.DispersionAmount = Mathf.Lerp (0.2f, 0.01f, t);

			yield return null;

		}


	}

	public void PlayHitPlayerAnimation () {

		StopCoroutine(HitPlayerAnimation ());
		StartCoroutine(HitPlayerAnimation ());

	}

	public IEnumerator KillPlayerAnimation () {

		ShakeCamera (0.15f, 0.15f, 0.3f);


		float t = 0;

		while (t < 1) {

			t += Time.deltaTime * 2.5f;

			ca.DispersionAmount = Mathf.Lerp (2f, 0.01f, t);

			yield return null;

		}


	}

	public void PlayKIllPlayerAnimation () {
	
		StopCoroutine(KillPlayerAnimation ());
		StartCoroutine(KillPlayerAnimation ());

	}

}
