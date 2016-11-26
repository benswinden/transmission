using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class VRSwitcher : MonoBehaviour {

	public bool LaunchAuthorVR = false;

	void Awake () {
		if (LaunchAuthorVR) {
			SceneManager.LoadScene("TrackVRRecorder");
		}
	}
	
}
