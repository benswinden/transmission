using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


[CustomEditor(typeof(Particle))]
public class ParticleEditor : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        Particle _target = (Particle)target;
        if (GUILayout.Button("Set Camera Follow")) {
            _target.setCameraFollow();
        }

    }


}