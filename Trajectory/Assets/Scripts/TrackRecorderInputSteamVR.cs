using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackRecorderInputSteamVR : TrackRecorderInput {

	//VR VERSION

	//offset from center of controller to draw line (pen tip)
	public Vector3 DrawOffset;

	//trigger down state
	private enum TriggerDownState {
		Controller1,
		Controller2,
		None
	}
	//store which controller's trigger started drawing
	private TriggerDownState DrawingController;

	
	void Awake() {
		Transform TR = GetComponent<Transform>();
		DrawingController = TriggerDownState.None;
		Init();
	}

	public void OnControllerPositionUpdate(ControllerPositions controllerPositions) {
		//update record tick
		RecordTick += Time.smoothDeltaTime;
		//RECORD Track
		if (IsRecording) {
			//wait for record delay
			if (RecordTick > RecordDelay) {
				RecordTick = 0;
				if (DrawingController == TriggerDownState.Controller1) {
					RecordTrackedPoint(controllerPositions.Controller1Position, controllerPositions.Controller1Rotation);
				} else if(DrawingController == TriggerDownState.Controller2) {
					RecordTrackedPoint(controllerPositions.Controller2Position, controllerPositions.Controller2Rotation);
				}
			}
		}
	}

	public void OnController1TriggerDown() {
		if (!IsRecording) {
			DrawingController = TriggerDownState.Controller1;
			StartVRRecording();
		}
	}
	public void OnController1TriggerUp() {
		if (IsRecording && DrawingController == TriggerDownState.Controller1) {
			DrawingController = TriggerDownState.None;
			StopVRRecording();
		}
	}
	public void OnController2TriggerDown() {
		if (!IsRecording) {
			DrawingController = TriggerDownState.Controller2;
			StartVRRecording();
		}
	}
	public void OnController2TriggerUp() {
		if (IsRecording && DrawingController == TriggerDownState.Controller2) {
			DrawingController = TriggerDownState.None;
			StopVRRecording();
		}
	}

	private void StartVRRecording() {
		if (!Active) {
			Activate();
		}
		StartTrackRecord();
		LastPoint = new Vector3(-10000f, 0, 0);
		IsRecording = true;
	}

	private void StopVRRecording() {
		StopTrackRecord();
	}

	private void RecordTrackedPoint(Vector3 position, Quaternion rotation){
		Vector3 drawPoint = position + rotation * DrawOffset;
		RecordTrackPoint(drawPoint);
		//display to screen
		CurrentTrackWorld.Add(drawPoint);
		TrackDisplay.DisplayLine(CurrentTrackWorld);
	}

}