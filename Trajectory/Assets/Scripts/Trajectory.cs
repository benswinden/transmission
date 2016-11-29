using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//which controller
public enum Controller {
	Controller1,
	Controller2,
	None
}

public class Trajectory : MonoBehaviour {

	public GameObject TrajectoryTrackPrefab;

	public Text DebugText;

	//offset from center of controller to draw line (pen tip)
	public Vector3 DrawOffset;

	//vr manager
	private GameObject VRManagerGO;
	private VRManager VRManager;

	//transform
	private Transform TR;

	//recorder
	private TrackRecorder Recorder;

	//current tracks (for each hand)
	private GameObject CurrentTrack1;
	private GameObject CurrentTrack2;
	private TrajectoryTrack CurrentTrajectoryTrack1;
	private TrajectoryTrack CurrentTrajectoryTrack2;

	//all tracks
	private List<TrajectoryTrack> TrajectoryTracks;

	//settings
	private int MaxTotalPoints = 1000;

	//state
	private int NumTotalPoints = 0;

	void Awake () {
		//cache references
		VRManagerGO = GameObject.Find("VRManager");
		VRManager = VRManagerGO.GetComponent<VRManager>();
		TR = GetComponent<Transform>();
		TrajectoryTracks = new List<TrajectoryTrack>();
		DebugText.text = MaxTotalPoints.ToString();
	}

	void Start() {
		//create recorder, listen for events
		Recorder = new TrackRecorder();
		Recorder.Init(DrawOffset);
		Recorder.OnRecordTrackStart += CreateNewTrack;
		Recorder.OnRecordTrackFinished += TrackRecordFinished;
		Recorder.OnRecordingTrack += UpdateCurrentTrack;
		//hook up vr inputs to recorder
		VRManager.OnController1TriggerDown += Recorder.OnController1TriggerDown;
		VRManager.OnController1TriggerUp += Recorder.OnController1TriggerUp;
		VRManager.OnController2TriggerDown += Recorder.OnController2TriggerDown;
		VRManager.OnController2TriggerUp += Recorder.OnController2TriggerUp;
		VRManager.OnControllerPositionUpdate += Recorder.OnControllerPositionUpdate;
		//VRManager.OnController1GripDown += OnController1GripDown;
		//VRManager.OnController2GripDown += OnController2GripDown;
	}

	void Update() {
		Recorder.Update();
		int l = TrajectoryTracks.Count;
		if (l > 0) {
			for (int i = 0; i < l; i++) {
				TrajectoryTracks[i].Advance();
			}
		}
	}

	private void CreateNewTrack(Controller whichController) {
		//create new recorded track
		GameObject CurrentTrack = Instantiate(TrajectoryTrackPrefab, Vector3.zero, Quaternion.identity, TR) as GameObject;
		if (whichController == Controller.Controller1) {
			CurrentTrajectoryTrack1 = CurrentTrack.GetComponent<TrajectoryTrack>();
		} else if(whichController == Controller.Controller2) {
			CurrentTrajectoryTrack2 = CurrentTrack.GetComponent<TrajectoryTrack>();
		}
	}

	private void TrackRecordFinished(Controller whichController) {
		if (whichController == Controller.Controller1) {
			CurrentTrajectoryTrack1.StartTrackPlayback();
			//add current track to list
			TrajectoryTracks.Add(CurrentTrajectoryTrack1);
		}
		if (whichController == Controller.Controller2) {
			CurrentTrajectoryTrack2.StartTrackPlayback();
			//add current track to list
			TrajectoryTracks.Add(CurrentTrajectoryTrack2);
		}
	}

	private void UpdateCurrentTrack(Controller whichController, Vector3 trackPoint) {
		if (whichController == Controller.Controller1) {
			CurrentTrajectoryTrack1.AddTrackPoint(trackPoint);
		}
		if (whichController == Controller.Controller2) {
			CurrentTrajectoryTrack2.AddTrackPoint(trackPoint);
		}
		NumTotalPoints++;
		DebugText.text = (MaxTotalPoints - NumTotalPoints).ToString();
	}

}
