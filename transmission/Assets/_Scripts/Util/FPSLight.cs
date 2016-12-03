using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSLight : MonoBehaviour {

    public bool startActive;
    public KeyCode toggleActiveKey;

    float deltaTime = 0.0f;
    string currentFPS;

    bool active;

    GameObject background;

    void Awake() {

        foreach (Transform child in transform.parent.transform) {
            if (child.name.Equals("Background"))
                background = child.gameObject;
        }        
    }

    void Start() {

        background.SetActive(false);
        GetComponent<Text>().text = "";

        if (startActive)
            toggleActive();        

        StartCoroutine("printFPS");
    }

    void Update() {

        if (Input.GetKeyUp(toggleActiveKey)) toggleActive();

        if (active) {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

            currentFPS = text;            
        }
    }

    void toggleActive() {

        if (!active) {

            active = true;
            background.SetActive(true);
        }
        else {

            active = false;
            background.SetActive(false);
            GetComponent<Text>().text = "";
        }
    }

    IEnumerator printFPS() {

        yield return new WaitForSeconds(1.0f);

        if (active) GetComponent<Text>().text = currentFPS;
        StartCoroutine("printFPS");
    }
}
