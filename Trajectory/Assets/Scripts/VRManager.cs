using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class VRManager : MonoBehaviour {

	//settings
	public bool DrawToScreen = true;

	//controller positions
	public delegate void ControllerPositionUpdate(ControllerPositions controllerPositions);
	public event ControllerPositionUpdate OnControllerPositionUpdate;
	private ControllerPositions CurrentControllerPositions;

	//controller1 trigger
	public delegate void Controller1TriggerDown();
	public event Controller1TriggerDown OnController1TriggerDown;
	public delegate void Controller1TriggerUp();
	public event Controller1TriggerUp OnController1TriggerUp;
	//controller2 trigger
	public delegate void Controller2TriggerDown();
	public event Controller2TriggerDown OnController2TriggerDown;
	public delegate void Controller2TriggerUp();
	public event Controller2TriggerUp OnController2TriggerUp;

	//controller1 pad click
	public delegate void Controller1PadButtonDown();
	public event Controller1PadButtonDown OnController1PadButtonDown;
	public delegate void Controller1PadButtonUp();
	public event Controller1PadButtonUp OnController1PadButtonUp;
	//controller2 pad click
	public delegate void Controller2PadButtonDown();
	public event Controller2PadButtonDown OnController2PadButtonDown;
	public delegate void Controller2PadButtonUp();
	public event Controller2PadButtonUp OnController2PadButtonUp;

	//controller1 grip
	public delegate void Controller1GripDown();
	public event Controller1TriggerDown OnController1GripDown;
	public delegate void Controller1GripUp();
	public event Controller1TriggerUp OnController1GripUp;
	//controller2 grip
	public delegate void Controller2GripDown();
	public event Controller2GripDown OnController2GripDown;
	public delegate void Controller2GripUp();
	public event Controller2GripUp OnController2GripUp;

	private Transform Controller1TR;
	private Transform Controller2TR;
	private Transform CurrentControllerTR;

	public SteamVR_TrackedObject TrackedObject1;
	public SteamVR_TrackedObject TrackedObject2;
	private SteamVR_Controller.Device Controller1{
		get{
			return SteamVR_Controller.Input((int)TrackedObject1.index);
		}
	}
	private SteamVR_Controller.Device Controller2{
		get{
			return SteamVR_Controller.Input((int)TrackedObject2.index);
		}
	}


	void Awake () {
		VRSettings.enabled = true;
		print("VR SETTINGS ENABLED");
		VRSettings.showDeviceView = DrawToScreen;
		Controller1TR = TrackedObject1.GetComponent<Transform>();
		Controller2TR = TrackedObject2.GetComponent<Transform>();
		//
		CurrentControllerPositions = new ControllerPositions();
	}

	void Update() {
		//SteamVR Trigger Down
		if (TrackedObject1.index != SteamVR_TrackedObject.EIndex.None && Controller1 != null && Controller1.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
			if(OnController1TriggerDown != null) {
				OnController1TriggerDown();
			}
		}
		if (TrackedObject2.index != SteamVR_TrackedObject.EIndex.None && Controller2 != null && Controller2.GetPressDown(SteamVR_Controller.ButtonMask.Trigger)) {
			if (OnController2TriggerDown != null) {
				OnController2TriggerDown();
			}
		}
		//SteamVR Trigger Up
		if (TrackedObject1.index != SteamVR_TrackedObject.EIndex.None && Controller1 != null && Controller1.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
			if (OnController1TriggerUp != null) {
				OnController1TriggerUp();
			}
		}
		if (TrackedObject2.index != SteamVR_TrackedObject.EIndex.None && Controller2 != null && Controller2.GetPressUp(SteamVR_Controller.ButtonMask.Trigger)) {
			if (OnController2TriggerUp != null) {
				OnController2TriggerUp();
			}
		}


		//SteamVR Touchpad Button Down
		if (TrackedObject1.index != SteamVR_TrackedObject.EIndex.None && Controller1 != null && Controller1.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
			float padX = Controller1.GetAxis().x;
			print(padX);
			if (OnController1PadButtonDown != null) {
				OnController1PadButtonDown();
			}
		}
		if (TrackedObject2.index != SteamVR_TrackedObject.EIndex.None && Controller2 != null && Controller2.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)) {
			if (OnController2PadButtonDown != null) {
				OnController2PadButtonDown();
			}
		}
		//SteamVR Touchpad Button Up
		if (TrackedObject1.index != SteamVR_TrackedObject.EIndex.None && Controller1 != null && Controller1.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) {
			if (OnController1PadButtonUp != null) {
				OnController1PadButtonUp();
			}
		}
		if (TrackedObject2.index != SteamVR_TrackedObject.EIndex.None && Controller2 != null && Controller2.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)) {
			if (OnController2PadButtonUp != null) {
				OnController2PadButtonUp();
			}
		}

		//SteamVR Grip Down
		if (TrackedObject1.index != SteamVR_TrackedObject.EIndex.None && Controller1 != null && Controller1.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
			if (OnController1GripDown != null) {
				OnController1GripDown();
			}
		}
		if (TrackedObject2.index != SteamVR_TrackedObject.EIndex.None && Controller2 != null && Controller2.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) {
			if (OnController2GripDown != null) {
				OnController2GripDown();
			}
		}
		//SteamVR Trigger Up
		if (TrackedObject1.index != SteamVR_TrackedObject.EIndex.None && Controller1 != null && Controller1.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
			if (OnController1GripUp != null) {
				OnController1GripUp();
			}
		}
		if (TrackedObject2.index != SteamVR_TrackedObject.EIndex.None && Controller2 != null && Controller2.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) {
			if (OnController2GripUp != null) {
				OnController2GripUp();
			}
		}

		//Stream controller positions
		if (OnControllerPositionUpdate != null) {
			CurrentControllerPositions.SetControllerPositions(Controller1TR, Controller2TR);
			OnControllerPositionUpdate(CurrentControllerPositions);
		}
	}

}

public struct ControllerPositions {
	public Vector3 Controller1Position;
	public Vector3 Controller2Position;
	public Quaternion Controller1Rotation;
	public Quaternion Controller2Rotation;
	public void SetControllerPositions(Transform controller1TR, Transform controller2TR) {
		Controller1Position = controller1TR.position;
		Controller2Position = controller2TR.position;
		Controller1Rotation = controller1TR.rotation;
		Controller2Rotation = controller2TR.rotation;
	}
	public override string ToString() {
		return "Controller1 position:" + Controller1Position + "\n" + "Controller2 position: " + Controller2Position;
	}
}