using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class AnimatorWait : MonoBehaviour {

    public float waitTime = 40;

    void Awake() {

        StartCoroutine("wait");
    }

    IEnumerator wait() {

        yield return new WaitForSeconds(waitTime);

        GetComponent<Animator>().SetTrigger("play");
    }

}