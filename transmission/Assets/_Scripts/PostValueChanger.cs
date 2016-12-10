using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class PostValueChanger : MonoBehaviour {

    public float value = 0;

    UnityEngine.PostProcessing.Utilities.PostProcessingController controller;

    void Awake() {

        controller = GetComponent<UnityEngine.PostProcessing.Utilities.PostProcessingController>();
    }

    void Update() {

        controller.bloom.bloom.intensity = value;
    }

}