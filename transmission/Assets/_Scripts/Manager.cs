using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Manager : MonoBehaviour {

    public Camera screenshotCamera;
    public KeyCode screenshotKey;
    public int screenshotSizeFactor = 3;

    [Space]

    public int gifWidth = 320;
    public int gifHeight = 240;

   
    string currentScene;            // Keep track of which level we are in for reloading



    void Awake() {
        
        currentScene = SceneManager.GetActiveScene().name;            

        // Create a folder
        if (!Application.isEditor) {

            System.IO.Directory.CreateDirectory(Application.dataPath + "/Screenshots");
        }
    }

	void Update() {

        if (Input.GetKeyUp(screenshotKey)) {
            takeScreenshot();
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
