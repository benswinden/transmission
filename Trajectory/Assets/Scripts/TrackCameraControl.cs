using UnityEngine;
using System.Collections;

public class TrackCameraControl : MonoBehaviour {

	private Transform TR;
	private Vector3 PositionOffset;
	private Vector3 TargetPosition;
	public float EaseMult = 0.2f;

	void Start () {
		//cache transform
		TR = GetComponent<Transform>();
		PositionOffset = TR.position;
		TargetPosition = Vector3.zero;
	}

	void LateUpdate (){
		TR.position = Vector3.Lerp(TR.position, TargetPosition + PositionOffset, Time.smoothDeltaTime * EaseMult);
	}

	public void SetCameraTarget(Vector3 targetPosition){
		TargetPosition = targetPosition;
	}

	public void ResetCamera(){
		TargetPosition = Vector3.zero;
		TR.position = PositionOffset;
	}
	
}
