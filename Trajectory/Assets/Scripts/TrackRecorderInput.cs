using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineDisplay))]
public class TrackRecorderInput : MonoBehaviour {

	//fires when recorder activates
	public delegate void RecorderActivated();
	public event RecorderActivated OnActivated;

	//fires when Track recording begins
	public delegate void RecordTrackStart();
	public event RecordTrackStart OnRecordTrackStart;

	//fires when Track recording ends
	public delegate void RecordTrackFinished(TrackData recordedTrack);
	public event RecordTrackFinished OnRecordTrackFinished;

	//currently drawn Track
	protected TrackData CurrentTrack;
	//current Track in world position
	protected List<Vector3> CurrentTrackWorld;
	//delay between recording points
	public float RecordDelay = 0;
	protected float RecordTick = 0;
	//threshold of distance to record new point
	protected float RecordThreshold = 0.001f;
	protected float CurrentRecordThreshold;
	protected Vector3 LastPoint;
	//maximum number of points in track
	public int MaxPoints = 500; 

	//active state
	protected bool Active = true;

	//is currently recording
	protected bool IsRecording = false;

	//line display
	protected LineDisplay TrackDisplay;

	//init data
	protected void Init() {
		print("TrackRecorderInput: INIT");
		//
		Transform TR = GetComponent<Transform>();
		//
		TrackDisplay = GetComponent<LineDisplay>();
		CurrentTrackWorld = new List<Vector3>();
	}

	//clean up and deactivate
	public virtual void Deactivate() {
		print("TrackRecorderInput: Deactivate");
		TrackDisplay.ClearLine();
		Active = false;
	}

	//reactivate
	public virtual void Activate() {
		print("TrackRecorderInput: Activate");
		Active = true;
		if (OnActivated != null) {
			OnActivated();
		}
	}

	//start Track recording
	protected void StartTrackRecord() {
		print("TrackRecorderInput: Start");
		//reset record threshold
		CurrentRecordThreshold = RecordThreshold;
		//create new Track
		CurrentTrack = new TrackData();
		//clear world Track poing list
		CurrentTrackWorld.Clear();
		if (OnRecordTrackStart != null) {
			OnRecordTrackStart();
		}
	}

	//Track recording finished
	protected void StopTrackRecord() {
		print("TrackRecorderInput: Finished");
		LastPoint = new Vector3(-10000f, 0, 0);
		IsRecording = false;
		if (CurrentTrack.TrackPoints.Count > 5) {
			print("Track RECORDER: Track recording successful");
			CurrentTrack.NormalizePosition();
			CurrentTrack.RecordFinished();
			if (OnRecordTrackFinished != null) {
				OnRecordTrackFinished(CurrentTrack);
			}
		} else {
			print("Track RECORDER: Not enough points to record Track, resetting");
			Reset();
		}
	}

	//record Track
	protected void RecordTrackPoint(Vector3 currentPoint) {
		CurrentTrack.WriteToTrack(currentPoint);
		if (CurrentTrack.TrackPoints.Count > 3) {
			CurrentRecordThreshold = 0;
		}
		if(CurrentTrack.TrackPoints.Count > MaxPoints) {
			StopTrackRecord();
		}
	}

	//reset recording
	protected void Reset() {

	}
	//

}
