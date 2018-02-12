using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	//this will be the parent all menu screens below it (but not the canvas which needs to be above it)
	public GameObject menuRoot;

    //main menu background
    public GameObject background;

	//this will hold all menu screens that need to be in the game
	public GameObject[] menuScreens;

	public Slider[] volumeSliders;
	public Toggle[] resolutionToggles;
	public int[] screenWidths;

	int activeScreenResIndex;

	public void Play () {
		CloseMenu ();
	}

	public void Quit () {
		Application.Quit ();
	}

	//turn on the main menu (this excludes in game menus)
	public void OpenMenu () {
		menuRoot.SetActive (true);
	}

	//close the main menu (this excludes in game menus)
	public void CloseMenu () {
		menuRoot.SetActive (false);
	}

	public void GoToScreen (string requestedScreenName) {

		//open the menu if it is not already open
		if (!menuRoot.activeSelf) OpenMenu ();

		//cycle through the menu screens and close the ones that are not requested
		for (int i = 0; i < menuScreens.Length; i++) {

			//show the screen if it has the name we are requesting
			if (menuScreens[i].name == requestedScreenName) {
				menuScreens[i].SetActive (true);

                //toggle background on or off depending on menu screen
                if(requestedScreenName == "Level_Select" || requestedScreenName == "Mode_Select") {
                    SetBackgroundActive(false);
                } else {
                    SetBackgroundActive(true);
                }

			//hide the screen if it has the name we are requesting
			} else {
				menuScreens[i].SetActive (false);
			}

		}
			
	}

    public void SetBackgroundActive(bool visible) {
        if (background == null)
            return;

        background.SetActive(visible);
    }

	public void SetScreenResolution (int i) {
		if (resolutionToggles[i].isOn) {
			activeScreenResIndex = i;
			float aspectRatio = 16 / 9f;
			Screen.SetResolution (screenWidths[i], (int)(screenWidths[i]/aspectRatio), false);
		}
	}

	public void SetFullScreen (bool isFullscreen) {
		for (int i = 0; i < resolutionToggles.Length; i++) {
			resolutionToggles[i].interactable = !isFullscreen;
		}

		if (isFullscreen) {
			Resolution[] allResolutions = Screen.resolutions;
			Resolution maxResolution = allResolutions [allResolutions.Length -1];
			Screen.SetResolution (maxResolution.width, maxResolution.height, true);
		} else {
			SetScreenResolution (activeScreenResIndex);
		}



	}
	 
	public void SetMasterVolume (float value) {
		AudioManager.instance.SetVolume (value, AudioManager.AudioChannel.Master);
	}

	public void SetMusicVolume (float value) {
		AudioManager.instance.SetVolume (value, AudioManager.AudioChannel.Music);
	}

	public void SetSFXVolume (float value) {
		AudioManager.instance.SetVolume (value, AudioManager.AudioChannel.SFX);
	}
		
}
