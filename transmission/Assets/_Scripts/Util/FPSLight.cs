using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSLight : MonoBehaviour {


    float deltaTime = 0.0f;
    string currentFPS;

    bool active = true;

    void Start() {

        StartCoroutine("printFPS");
    }

    void Update() {

        if (active) {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

            currentFPS = text;            
        }
    }

    IEnumerator printFPS() {

        yield return new WaitForSeconds(1.0f);

        GetComponent<Text>().text = currentFPS;
        StartCoroutine("printFPS");
    }
}
