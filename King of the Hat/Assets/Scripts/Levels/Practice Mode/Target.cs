using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {

		Init ();
		
	}
	
	// Update is called once per frame
	void Update () {

		Move ();
		
	}

//	void OnTriggerEnter2D (Collider2D other) {
//
//		if (other.CompareTag ("Hat")) {
//
//			Hat hat = other.GetComponent <Hat> ();
//
//			if (hat.isBeingThrown) {
//
//				Remove ();
//
//			}
//
//		}
//
//		print ("hit tough");
//
//	}

	public virtual void Init () {



	}

	public virtual void Move () {



	}

	public virtual void Remove () {

		Destroy (gameObject);
		//print ("destroyed");

	}

}
