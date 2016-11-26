using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackRecorder {

	//delay between recording points
	private float RecordDelay = 0.01f;
	private float RecordTick = 0;
	//threshold of distance to record new point
	private float RecordThreshold = 0.001f;
	private float CurrentRecordThreshold;
	private Vector3 LastPoint;
	//maximum number of points in track
	public int MaxPoints = 500;
	//is currently recording
	protected bool IsController1Recording = false;
	protected bool IsController2Recording = false;
	//draw offset (pen tip)
	private Vector3 DrawOffset;

	//fires when Track recording begins
	public delegate void RecordTrackStart(Controller whichController);
	public event RecordTrackStart OnRecordTrackStart;
	//Track recording ends
	public delegate void RecordTrackFinished(Controller whichController);
	public event RecordTrackFinished OnRecordTrackFinished;
	//broadcast track data during record
	public delegate void RecordingTrack(Controller whichController, Vector3 trackPoint);
	public event RecordingTrack OnRecordingTrack;

	//VR input
	public void OnController1TriggerDown() {
		if (!IsController1Recording) {
			IsController1Recording = true;
			if (OnRecordTrackStart != null) {
				OnRecordTrackStart(Controller.Controller1);
			}
		}
	}
	public void OnController1TriggerUp() {
		if (IsController1Recording) {
			IsController1Recording = false;
			if (OnRecordTrackFinished != null) {
				OnRecordTrackFinished(Controller.Controller1);
			}
		}
	}
	public void OnController2TriggerDown() {
		if (!IsController2Recording) {
			IsController2Recording = true;
			if (OnRecordTrackStart != null) {
				OnRecordTrackStart(Controller.Controller2);
			}
		}
	}
	public void OnController2TriggerUp() {
		if (IsController2Recording) {
			IsController2Recording = false;
			if (OnRecordTrackFinished != null) {
				OnRecordTrackFinished(Controller.Controller2);
			}
		}
	}
	public void OnControllerPositionUpdate(ControllerPositions controllerPositions) {
		//update record tick
		RecordTick += Time.smoothDeltaTime;
		//RECORD Track
		Vector3 drawPoint;
		if (RecordTick > RecordDelay) {
			RecordTick = 0;
			if (IsController1Recording) {
				drawPoint = controllerPositions.Controller1Position + controllerPositions.Controller1Rotation * DrawOffset;
				if (OnRecordingTrack != null) {
					OnRecordingTrack(Controller.Controller1, drawPoint);
				}
			}
			if (IsController2Recording) {
				drawPoint = controllerPositions.Controller2Position + controllerPositions.Controller2Rotation * DrawOffset;
				if (OnRecordingTrack != null) {
					OnRecordingTrack(Controller.Controller2, drawPoint);
				}
			}
		}
	}

	public void Init(Vector3 drawOffset) {
		DrawOffset = drawOffset;
	}

	public void Update() {

	}

}
