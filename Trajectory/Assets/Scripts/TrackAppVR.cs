using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackAppVR : TrackApp {

	protected TrackRecorderInputSteamVR Recorder;
	//vr manager
	private GameObject VRManagerGO;
	private VRManager VRManager;

	protected override void CacheReferences() {
		base.CacheReferences();
		Recorder = RecorderGO.GetComponent<TrackRecorderInputSteamVR>();
	}

	protected override void Init () {

		print("TrackAPPVR: Start");
		//cache reference to vr manager
		VRManagerGO = GameObject.Find("VRManager");
		VRManager = VRManagerGO.GetComponent<VRManager>();
		//hook up vr inputs to recorder
		VRManager.OnController1TriggerDown += Recorder.OnController1TriggerDown;
		VRManager.OnController1TriggerUp += Recorder.OnController1TriggerUp;
		VRManager.OnController2TriggerDown += Recorder.OnController2TriggerDown;
		VRManager.OnController2TriggerUp += Recorder.OnController2TriggerUp;
		VRManager.OnController1GripDown += OnController1GripDown;
		VRManager.OnController2GripDown += OnController2GripDown;
		//listen for recorder activation
		Recorder.OnActivated += RecorderActivated;
		//listen for finish of recording
		Recorder.OnRecordTrackFinished += RecorderFinished;
		//activate recorder
		Recorder.Activate();
		base.Init();
	}

	protected void OnController1GripDown() {
		Playback.TogglePaused();
	}

	protected void OnController2GripDown() {
		Pause();
	}

	protected override void RecorderActivated(){
		print("TrackAPPVR: Recorder activating");
		base.RecorderActivated();
		//deactivate playback to start recording
		VRManager.OnController1PadButtonDown -= Playback.OnApproveTrackPressed;
		VRManager.OnController2PadButtonDown -= Playback.OnApproveTrackPressed;
		//stream controller data to recorder
		VRManager.OnControllerPositionUpdate += Recorder.OnControllerPositionUpdate;
	}

	protected override void RecorderFinished(TrackData recordedTrack){
		base.RecorderFinished(recordedTrack);
		//Track recorded, deactivate recorder and start Track playback
		Recorder.Deactivate();
		//stop streaming controller data to recorder
		VRManager.OnControllerPositionUpdate -= Recorder.OnControllerPositionUpdate;
		//hook up pad for playback inputs
		VRManager.OnController1PadButtonDown += Playback.OnApproveTrackPressed;
		VRManager.OnController2PadButtonDown += Playback.OnApproveTrackPressed;
		//approve gesture immediately
		Playback.OnApproveTrackPressed();
	}

}
