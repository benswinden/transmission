using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Manager : MonoBehaviour {

    public static CameraFollow camera;

    public Camera screenshotCamera;
    public KeyCode screenshotKey;
    public int screenshotSizeFactor = 3;

    [Space]

    public bool hideDebugMeshes;
    public List<GameObject> debugMeshes;

    //public int gifWidth = 320;
    //public int gifHeight = 240;       

    [HideInInspector]
    public float timeScale;


    string currentScene;            // Keep track of which level we are in for reloading


    void Awake() {

        timeScale = Time.timeScale;

        currentScene = SceneManager.GetActiveScene().name;            

        // Create a folder
        if (!Application.isEditor) {

            System.IO.Directory.CreateDirectory(Application.dataPath + "/Screenshots");
        }
    }

    void Start() {
        
        // Hides any meshes in this list, if it's not a mesh, looks to that objects children for meshes to hide
        if (hideDebugMeshes) {
            foreach (GameObject obj in debugMeshes) {

                if (obj.GetComponent<MeshRenderer>())
                    obj.GetComponent<MeshRenderer>().enabled = false;
                else {
                    foreach (Transform child in obj.transform) {

                        if (child.GetComponent<MeshRenderer>())
                            child.GetComponent<MeshRenderer>().enabled = false;
                    }
                }

            }
        }
    }

	void Update() {

        if (Time.timeScale != timeScale) Time.timeScale = timeScale;

        if (Input.GetKeyUp(screenshotKey)) {
            takeScreenshot();
        }

        if (Input.GetKeyUp(KeyCode.R)) {

            SceneManager.LoadScene(0);
        }

        if (SceneManager.GetActiveScene().name != currentScene) {
            currentScene = SceneManager.GetActiveScene().name;            
        }  
	}

    public void reloadScene() {
        
        SceneManager.LoadScene(currentScene);                
    }


    // Utitilies
        // Screenshots
    public void takeScreenshot() {

        Application.CaptureScreenshot(ScreenShotName(), screenshotSizeFactor);
    }


    public string ScreenShotName() {

        return string.Format("{0}/Screenshots/{1}.png", 
            Application.dataPath,
            System.DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss"));
     }

    // Gifs


    public string gifName() {

        return string.Format("{0}/Gifs/{1}.gif",
            Application.dataPath,
            System.DateTime.Now.ToString("yyyy-MM-dd__HH-mm-ss"));
    }
}
