using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR;

public class TrackApp : MonoBehaviour {

	//recorder
	protected GameObject RecorderGO;
	//playback
	protected GameObject PlaybackGO;
	protected TrackPlayback Playback;

	//visualizer events
	public delegate void DeactivateVisualizer();
	public event DeactivateVisualizer OnDeactivateVisualizer;
	public delegate void StreamTrackPoints(List<Vector3> trackPoints);
	public event StreamTrackPoints OnStreamTrackPoints;
	public delegate void PauseDelegate(bool pauseState);
	public event PauseDelegate OnPause;

	protected bool Paused = false;

	protected TrackData CurrentTrack;

	public void Awake() {
		CacheReferences();
	}

	public void Start() {
		Init();
	}

	public virtual void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Pause();
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			var path = Application.dataPath + "/SavedTracks/track" +
				System.DateTime.Now.ToString("_yyyy-MM-dd_")
				+ System.DateTime.Now.ToString("hh-mm-ss")
				+ ".txt";
			print("TrackApp: Saving track: " + path);
			ES2.Save(CurrentTrack, path);
		}
	}

	protected void Pause() {
		Paused = !Paused;
		if (OnPause != null) {
			OnPause(Paused);
		}
		Playback.Pause(Paused);
	}

	protected void UnPause() {
		Paused = false;
		if (OnPause != null) {
			OnPause(Paused);
		}
		Playback.Pause(Paused);
	}

	protected virtual void CacheReferences() {
		//get reference to recorder
		RecorderGO = GameObject.Find("TrackRecorder");
		//get reference to playback
		PlaybackGO = GameObject.Find("TrackPlayback");
		Playback = PlaybackGO.GetComponent<TrackPlayback>();
	}

	protected virtual void Init() {
		print("TrackAPP: Start");
		//deactivate playback on start
		Playback.enabled = false;
		if (Playback.HasLoadedTrack()) {
			string path = Application.dataPath + "/SavedTracks/" + Playback.LoadedTrackName + ".txt";
			print("TrackApp: Loading track from " + path);
			CurrentTrack = ES2.Load<TrackData>(path);
			RecorderFinished(CurrentTrack);
		}
	}

	protected virtual void RecorderActivated() {
		UnPause();
		//deactivate playback to start recording
		Playback.Deactivate();
		//cancel track stream
		Playback.OnStreamTrackPoints -= ReceiveTrackPoints;
		//deactivate visualizer
		if (OnDeactivateVisualizer != null) {
			OnDeactivateVisualizer();
		}
	}

	protected virtual void RecorderFinished(TrackData recordedTrack) {
		CurrentTrack = recordedTrack;
		UnPause();
		//enable playback and start playing
		Playback.enabled = true;
		Playback.StartPlayback(recordedTrack);
		//listen for track stream
		Playback.OnStreamTrackPoints += ReceiveTrackPoints;
	}

	protected void ReceiveTrackPoints(List<Vector3> trackPoints) {
		if (OnStreamTrackPoints != null) {
			OnStreamTrackPoints(trackPoints);
		}
	}

	public void SetMinTrackPoints(int minTrackPoints) {
		Playback.SetMinTrackPoints(minTrackPoints);
	}

}
