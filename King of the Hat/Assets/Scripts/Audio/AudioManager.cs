using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public enum AudioChannel {Master, SFX, Music};

	public static AudioManager instance;

	float masterVolumePercent = 1f;
	float sfxVolumePercent = 1f;
	float musicVolumePercent = 0f;

	AudioSource [] musicSources;
	int activeMusicSourceIndex;

	SFXLibrary library;

	void Awake () {

		if (instance != null) {
			
			Destroy (gameObject);

		} else {

			instance = this;
			DontDestroyOnLoad (this);

			musicSources = new AudioSource[2];
			for (int i = 0; i < 2; i++) {
				GameObject newMusicSource = new GameObject ("Music_Source_" + "0" + (i + 1));
				musicSources[i] = newMusicSource.AddComponent<AudioSource> ();
				newMusicSource.transform.parent = this.transform;
			}

			library = GetComponent <SFXLibrary> ();
			
		}

	}

	public void SetVolume (float volumePercent, AudioChannel channel) {
		switch (channel) {
		case AudioChannel.Master:
			masterVolumePercent = volumePercent;
			break;

		case AudioChannel.Music:
			musicVolumePercent = volumePercent;
			break;

		case AudioChannel.SFX:
			sfxVolumePercent = volumePercent;
			break;

		default:
			break;
		}

		for (int i = 0; i < musicSources.Length; i++) {
			musicSources[i].volume = musicVolumePercent * masterVolumePercent;
		}
	}

	public void PlayMusic (AudioClip clip, float fadeDuration = 1) {
		activeMusicSourceIndex = 1 - activeMusicSourceIndex;
		musicSources[activeMusicSourceIndex].clip = clip;
		musicSources[activeMusicSourceIndex].Play ();

		StartCoroutine (AnimateMusicCrossFade (fadeDuration));

	}

	public void PlaySound (AudioClip clip, Vector3 pos) {
		if (clip != null)
			AudioSource.PlayClipAtPoint (clip, pos, sfxVolumePercent * masterVolumePercent);
	}

	public void PlaySound (string soundName, Vector3 pos) {
		PlaySound (library.GetClipFromName (soundName), pos);
	}

	IEnumerator AnimateMusicCrossFade (float duration) {
		float percent = 0;

		while (percent < 1) {
			percent += Time.deltaTime * 1/duration;
			musicSources[activeMusicSourceIndex].volume = Mathf.Lerp (0, musicVolumePercent * masterVolumePercent, percent);
			musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp (musicVolumePercent * masterVolumePercent, 0, percent);

			yield return null;
		}
	}

}
