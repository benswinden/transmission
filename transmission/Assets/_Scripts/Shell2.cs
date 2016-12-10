using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Shell2 : MonoBehaviour {


    void Awake() {

        StartCoroutine("routine");
    }

    void Update() {

        GetComponent<ShellRenderer>().noiseAmplitude += 0.000042f;
    }

    IEnumerator routine() {

        yield return new WaitForSeconds(120);

        Destroy(this);
    }
}