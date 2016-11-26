using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//data for each Track
public class TrackData {

	//list of points
	public List<Vector3> PointList;
	//trim start/end
	private float TrimStart = 0;
	private float TrimEnd = 1f;
	//world center when drawn
	public Vector3 _WorldCenter;

	public TrackData() {
		PointList = new List<Vector3>();
	}

	//add to Track
	public void WriteToTrack(Vector3 currentPoint) {
		PointList.Add(currentPoint);
	}

	//list of Track points
	public List<Vector3> TrackPoints
	{
		get
		{
			return PointList;
		}
	}

	public void NormalizePosition() {
		//center position of all points based on average position
		Vector3 average = Vector3.zero;
		for (int i = 0; i < PointList.Count; i++) {
			average += PointList[i];
		}
		average /= PointList.Count;
		_WorldCenter = average;
		for (int i = 0; i < PointList.Count; i++) {
			PointList[i] -= _WorldCenter;
		}
	}

	public void RecordFinished() {
		for (int i = 0; i < 1; i++) {
			TrackPoints.RemoveAt(TrackPoints.Count - 1);
		}
	}

	public Vector3 WorldCenter
	{
		get
		{
			return _WorldCenter;
		}
	}

	public override string ToString() {
		string pointListOutput = "";
		for (int i = 0; i < PointList.Count; i++) {
			pointListOutput += PointList[i];
			pointListOutput += "  ";
		}
		return "TrackData: " + pointListOutput;
	}

}
