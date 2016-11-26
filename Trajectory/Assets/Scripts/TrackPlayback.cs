using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineDisplay))]
public class TrackPlayback : MonoBehaviour {

	//cache transform
	private Transform TR;

	//broadcasts center point
	public delegate void StartTrackStreaming();
	public event StartTrackStreaming OnStartTrackStreaming;
	public delegate void StreamTrackPoints(List<Vector3> trackPoints);
	public event StreamTrackPoints OnStreamTrackPoints;

	//broadcasts center point
	public delegate void PlaybackUpdate(List<Vector3> trackPoints, Vector3 centerPoint);
	public event PlaybackUpdate OnPlaybackUpdate;

	//Track to play back
	private TrackData PlaybackTrack;
	private List<Vector3> PlaybackTrackPointBuffer;
	
	//line display
	protected LineDisplay TrackDisplay;

	private List<Vector3> PlaybackLinePoints;
	
	//scale to multiply normalized Track line to world units
	//private float TrackSegmentScale = 16f;
	private float TrackSegmentScale = 1f;
	private float TrackSegmentScaleTarget = 1f;

	//number of points to display onscreen at once
	private int _NumDisplayPoints;

	//number of points to move line per update
	private int NumStepsPerUpdate = 1;

	//delay before updating
	private float UpdateDelay = 0f;
	private float UpdateDelayTick = 0;
	
	//debug points
	private List<Vector3> DebugTrackStartPoints;
	private List<Vector3> DebugTrackStartDirPoints;
	private List<Vector3> DebugTrackEndDirPoints;

	//active state
	private bool Active = false;

	//debug state
	private bool DebugVis = false;

	//pause state
	private bool MasterPaused = false;
	private bool Paused = false;
	private bool PauseStateAtMasterPause;

	//centering offset
	private Vector3 DrawCenterOffset;
	private bool CenterTrack = false;
	private Vector3 CenteringPoint;
	private Vector3 CenteringOffset;
	private Vector3 CenteringOffsetTarget;

	//stream track points
	private bool IsStreamingTrack = false;

	//minimum track points
	private int MinTrackPoints = 0;

	//whether to start stepping line at start
	public bool PauseOnStart = false;

	//
	public TextAsset LoadedTrack;
		

	void Awake(){
		TR = GetComponent<Transform>();
		CenteringPoint = TR.position;
		TrackDisplay = GetComponent<LineDisplay>();
		if(DebugVis){
			DebugTrackStartPoints = new List<Vector3>();
			DebugTrackStartDirPoints = new List<Vector3>();
			DebugTrackEndDirPoints = new List<Vector3>();
		}
		PlaybackLinePoints = new List<Vector3>();
	}

	void Start () {
		print("TrackPLAYBACK: Start");
	}

	public bool HasLoadedTrack() {
		return !(LoadedTrack == null);
	}

	public string LoadedTrackName
	{
		get
		{
			return LoadedTrack.name;
		}
	}


	void Update () {
		//update line
		UpdateLine();
		//DEBUG
		if(DebugVis){
			for(int i = 0; i < DebugTrackStartPoints.Count; i++){
				Debug.DrawLine(Vector3.zero, DebugTrackStartPoints[i] * TrackSegmentScale, Color.cyan);
			}
			for(int i = 0; i < DebugTrackStartDirPoints.Count; i += 2){
				//Debug.DrawRay(DebugTrackStartDirPoints[i] * TrackSegmentScale, DebugTrackStartDirPoints[i+1] * 3f, Color.magenta);
			}
			for(int i = 0; i < DebugTrackEndDirPoints.Count; i += 2){
				Debug.DrawRay(DebugTrackEndDirPoints[i] * TrackSegmentScale, DebugTrackEndDirPoints[i+1] * 1f, Color.green);
			}
		}
		if(Input.GetKeyDown(KeyCode.L)){
			TogglePaused();
		}
	}

	public void TogglePaused() {
		if (!MasterPaused) {
			Paused = !Paused;
		}
	}

	public void Pause(bool pauseState) {
		MasterPaused = pauseState;
		if(pauseState) PauseStateAtMasterPause = Paused;
		if (MasterPaused) {
			Paused = true;
		} else if (!MasterPaused && !PauseStateAtMasterPause) {
			Paused = false;
		}
	}

	public void StepLine(){
		//remove points from start of array
		for(int i = 0; i < NumStepsPerUpdate; i++){
			if(PlaybackTrackPointBuffer.Count > 0){
				PlaybackTrackPointBuffer.RemoveAt(0);
			}
		}
		//if buffer point count is less than number of display points append Tracks until full
		List<Vector3> CurrentTrackPoints = PlaybackTrack.TrackPoints;
		while(PlaybackTrackPointBuffer.Count < _NumDisplayPoints){
			AppendTrack(CurrentTrackPoints);
		}
		//display line from buffer
		PlaybackLinePoints.Clear();
		for (int i = 0; i < _NumDisplayPoints; i++){
			PlaybackLinePoints.Add(PlaybackTrackPointBuffer[i] * TrackSegmentScale + DrawCenterOffset);
		}
		if (CenterTrack) {
			CenteringOffsetTarget = (CenteringPoint - PlaybackLineCenter);
			CenteringOffset = Vector3.Lerp(CenteringOffset, CenteringOffsetTarget, Time.smoothDeltaTime * 5f);
			TrackSegmentScale = Mathf.Lerp(TrackSegmentScale, TrackSegmentScaleTarget, Time.smoothDeltaTime * 5f);
			for (int i = 0; i < _NumDisplayPoints; i++) {
				PlaybackLinePoints[i] += CenteringOffset;
			}
		}
	}

	public void UpdateLine(){
		if(Active){
			UpdateDelayTick += Time.smoothDeltaTime;
			if(!Paused){
				StepLine();
			}
			if(UpdateDelayTick > UpdateDelay){
				//broadcast Track and center point
				if (IsStreamingTrack) {
					if(OnStreamTrackPoints != null) {
						OnStreamTrackPoints(PlaybackLinePoints);
					}
					if (TrackDisplay.LineActive) {
						TrackDisplay.ClearLine();
					}
				} else {
					//display track
					TrackDisplay.DisplayLine(PlaybackLinePoints);
				}
				//TrackDisplay.DisplayLine(PlaybackLinePoints);
				UpdateDelayTick = 0;
			}
		}
	}

	public void StartPlayback(TrackData recordedTrack){
		print("TrackPLAYBACK: Starting Track playback, Track: " + recordedTrack);
		//clear debug
		if(DebugVis){
			DebugTrackStartPoints.Clear();
			DebugTrackStartDirPoints.Clear();
			DebugTrackEndDirPoints.Clear();
		}
		//store current Track
		PlaybackTrack = recordedTrack;
		//set initial playback Track points
		PlaybackTrackPointBuffer = new List<Vector3>(recordedTrack.TrackPoints);
		//get number of points in Track list, set to either count or minimum track points
		_NumDisplayPoints = (PlaybackTrackPointBuffer.Count < MinTrackPoints) ? MinTrackPoints : PlaybackTrackPointBuffer.Count;
		//append one Track to end
		AppendTrack(recordedTrack.TrackPoints);
		//activate
		Active = true;
		//reset centering
		CenterTrack = false;
		TrackSegmentScaleTarget = 1f;
		TrackSegmentScale = 1f;
		DrawCenterOffset = PlaybackTrack.WorldCenter;
		//don't stream the track yet, wait for approval
		IsStreamingTrack = false;
		//step line once
		StepLine();
		//and pause if we need to pause on start
		Pause(PauseOnStart);
		if (PauseOnStart) {
			StartTrackStream();
		}
	}

	private void AppendTrack(List<Vector3> TrackPoints){
		//
		//transform points to end of current Track
		//
		Vector3 firstPoint = TrackPoints[0];
		Vector3 startDirection = (TrackPoints[1] - TrackPoints[0]).normalized;
		Vector3 lastPoint = PlaybackTrackPointBuffer[PlaybackTrackPointBuffer.Count-1];
		Vector3 endDirection = (lastPoint - PlaybackTrackPointBuffer[PlaybackTrackPointBuffer.Count-2]).normalized;
		//make sure we're not using overlapping points to get start and end direction, loop through buffer until unique point is found
		int startDirectionI = 2;
		while(startDirection.sqrMagnitude == 0 && startDirectionI < PlaybackTrackPointBuffer.Count - 1){
			startDirection = (TrackPoints[startDirectionI] -TrackPoints[0]).normalized;
			startDirectionI++;
		}
		int endDirectionI = 3;
		while(endDirection.sqrMagnitude == 0 && endDirectionI < PlaybackTrackPointBuffer.Count - 2){
			endDirection = (lastPoint - PlaybackTrackPointBuffer[PlaybackTrackPointBuffer.Count - endDirectionI]).normalized;
			endDirectionI++;
		}
		//offset to end of line
		Vector3 startOffset = lastPoint - firstPoint;
		//DEBUG
		if(DebugVis){
			DebugTrackStartPoints.Add (firstPoint + startOffset);
			DebugTrackStartDirPoints.Add (firstPoint + startOffset);
			DebugTrackStartDirPoints.Add (startDirection);
			DebugTrackEndDirPoints.Add (lastPoint);
			DebugTrackEndDirPoints.Add (endDirection);
		}
		//rotate line around end direction axis by random amount each time we append Track
		int l = TrackPoints.Count;
		Quaternion rndAxisRotation = Quaternion.AngleAxis(Random.Range(0, 360f), endDirection);
		for(int i = 0; i < l; i++){
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
			PlaybackTrackPointBuffer.Add(point);
		}
	}

	private Vector3 PlaybackLineCenter{
		get {
			Vector3 centerPoint = Vector3.zero;
			for (int i = 0; i < PlaybackLinePoints.Count; i++) {
				centerPoint += PlaybackLinePoints[i];
			}
			centerPoint /= PlaybackLinePoints.Count;
			return centerPoint;
		}
	}

	public void SetMinTrackPoints (int minTrackPoints) {
		MinTrackPoints = minTrackPoints;
	}

	public void OnApproveTrackPressed() {
		print("Playback: Track Approved");
		if (!CenterTrack) {
			CenterTrack = true;
			CenteringOffset = Vector3.zero;
			CenteringOffsetTarget = Vector3.zero;
			TrackSegmentScaleTarget = 1f;
			Invoke("StartTrackStream", 0.05f);
		} else {
			CenterTrack = false;
			DrawCenterOffset += CenteringOffset;
		}
	}

	public void StartTrackStream() {
		IsStreamingTrack = true;
		if(OnStartTrackStreaming != null) {
			OnStartTrackStreaming();
		}
		if (PauseOnStart) {
			Pause(true);
		}
	}

	public void Deactivate(){
		Active = false;
		if(PlaybackTrackPointBuffer != null) {
			PlaybackTrackPointBuffer.Clear();
		}
		TrackDisplay.ClearLine();
	}

	public int NumDisplayPoints {
		get {
			return _NumDisplayPoints;
		}
	}

}
