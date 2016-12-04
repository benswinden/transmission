using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;


[CustomEditor(typeof(Manager))]
public class ManagerEditor : Editor{

    SerializedProperty timeScale;

    void OnEnable() {
        
        timeScale = serializedObject.FindProperty("timeScale");        
    }

    public override void OnInspectorGUI() {

        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.Slider(timeScale, 0, 1, new GUIContent("TimeScale"));
        //ProgressBar(timeScale.floatValue, "Damage");

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }

    // Custom GUILayout progress bar.
    void ProgressBar(float value, string label) {

        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }

}