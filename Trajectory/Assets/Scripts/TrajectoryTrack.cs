using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryTrack : MonoBehaviour {

	//track data
	private TrackData TrackData;

	private List<Vector3> PlaybackPointBuffer;
	private List<Vector3> PlaybackLinePoints;

	private int NumDisplayPoints;
	private int NumStepsPerUpdate = 1;

	//line display
	private LineDisplay TrackDisplay;

	private bool _IsPlaying = false;

	void Awake () {
		TrackDisplay = GetComponent<LineDisplay>();
		PlaybackLinePoints = new List<Vector3>();
		TrackData = new TrackData();
	}

	public void Advance() {
		StepLine();
	}

	public void AddTrackPoint(Vector3 newPoint) {
		TrackData.WriteToTrack(newPoint);
		TrackDisplay.DisplayLine(TrackData.TrackPoints);
	}

	public void StartTrackPlayback() {
		_IsPlaying = true;
		//
		TrackDisplay.ClearLine();
		//set initial playback Track points
		PlaybackPointBuffer = new List<Vector3>(TrackData.TrackPoints);
		//get number of points in Track list, set to either count or minimum track points
		NumDisplayPoints = PlaybackPointBuffer.Count;
		//append one Track to end
		AppendTrack();
		//step line once
		StepLine();
	}

	public void StepLine() {
		//remove points from start of array
		for (int i = 0; i < NumStepsPerUpdate; i++) {
			if (PlaybackPointBuffer.Count > 0) {
				PlaybackPointBuffer.RemoveAt(0);
			}
		}
		//if buffer point count is less than number of display points append Tracks until full
		List<Vector3> CurrentTrackPoints = TrackData.TrackPoints;
		while (PlaybackPointBuffer.Count < NumDisplayPoints) {
			AppendTrack();
		}
		//display line from buffer
		PlaybackLinePoints.Clear();
		for (int i = 0; i < NumDisplayPoints; i++) {
			PlaybackLinePoints.Add(PlaybackPointBuffer[i]);
		}
		TrackDisplay.DisplayLine(PlaybackLinePoints);
	}

	private void AppendTrack() {
		//
		//transform points to end of current Track
		//
		List<Vector3> TrackPoints = TrackData.TrackPoints;
		Vector3 firstPoint = TrackPoints[0];
		Vector3 startDirection = (TrackPoints[1] - TrackPoints[0]).normalized;
		Vector3 lastPoint = PlaybackPointBuffer[PlaybackPointBuffer.Count - 1];
		Vector3 endDirection = (lastPoint - PlaybackPointBuffer[PlaybackPointBuffer.Count - 2]).normalized;
		//make sure we're not using overlapping points to get start and end direction, loop through buffer until unique point is found
		int startDirectionI = 2;
		while (startDirection.sqrMagnitude == 0 && startDirectionI < PlaybackPointBuffer.Count - 1) {
			startDirection = (TrackPoints[startDirectionI] - TrackPoints[0]).normalized;
			startDirectionI++;
		}
		int endDirectionI = 3;
		while (endDirection.sqrMagnitude == 0 && endDirectionI < PlaybackPointBuffer.Count - 2) {
			endDirection = (lastPoint - PlaybackPointBuffer[PlaybackPointBuffer.Count - endDirectionI]).normalized;
			endDirectionI++;
		}
		//offset to end of line
		Vector3 startOffset = lastPoint - firstPoint;
		//rotate line around end direction axis by random amount each time we append Track
		int l = TrackPoints.Count;
		Quaternion rndAxisRotation = Quaternion.AngleAxis(Random.Range(0, 360f), endDirection);
		for (int i = 0; i < l; i++) {
			Vector3 point = TrackPoints[i];
			//direction relative to first point pivot
			Vector3 dir = point - firstPoint;
			//rotate point around pivot
			dir = Quaternion.FromToRotation(startDirection, endDirection) * dir;
			//rotate random degress around end dir axis
			dir = rndAxisRotation * dir;
			//translate to pivot
			point = dir + firstPoint;
			//translate point to end
			point += startOffset;
			//add point to end of buffer
			PlaybackPointBuffer.Add(point);
		}
	}

	public int NumPoints
	{
		get
		{
			return TrackData.TrackPoints.Count;
		}
	}

	public bool IsPlaying
	{
		get
		{
			return _IsPlaying;
		}
	}

}
