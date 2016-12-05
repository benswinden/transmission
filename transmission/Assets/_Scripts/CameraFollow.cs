using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.PostProcessing.Utilities;


public class CameraFollow : MonoBehaviour {

    public float dampTime = 0.15F;
    public GameObject target;
    public float distanceFromObject = 1;

    [Space]

    public GameObject focusPoint;

    GameObject _target;
    FocusPuller focusPuller;
    private Vector3 velocity = Vector3.zero;


    void Awake() {

        Manager.camera = this;

        focusPuller = GetComponentInChildren<FocusPuller>();
    }

    void Update() {

        if (target != _target) {
            setTarget();
        }

        if (target != null) {

            var targetPos = target.transform.position + ( -Vector3.forward * distanceFromObject );

            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, dampTime);
        }
    }

    public void setTarget() {

        _target = target;

        if (target == null)
            focusPuller.target = focusPoint.transform;
        else
            focusPuller.target = target.transform;
    }

    public void setTarget(GameObject targ) {

        target = targ;
        _target = targ;

        if (target == null)
            focusPuller.target = focusPoint.transform;
        else
            focusPuller.target = targ.transform;
    }
}