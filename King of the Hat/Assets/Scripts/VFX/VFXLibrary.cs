using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXLibrary : MonoBehaviour {

	public ParticleGroup[] particleGroups;

	Dictionary<string, ParticleSystem> groupDictionary = new Dictionary<string, ParticleSystem> ();

	void Awake () {
	
		foreach (ParticleGroup particleGroup in particleGroups) {
			groupDictionary.Add (particleGroup.groupID, particleGroup.system);
		}

	}

	public ParticleSystem GetParticleSystemFromName (string name) {
		if (groupDictionary.ContainsKey (name)) {
			ParticleSystem system = groupDictionary [name];
			system.gameObject.SetActive (true);
			return system;
		}
		return null;
	}

	[System.Serializable]
	public class ParticleGroup {
		public string groupID;
		public ParticleSystem system;
	}

}
