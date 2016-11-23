using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emitter : MonoBehaviour {

    public GameObject meshObject;
    public GameObject obj;

    [Space]

    public float waitTime = 0.5f;


    List<Vector3> vertexList = new List<Vector3>();


    void Awake() {

        var verticeList = meshObject.GetComponent<MeshFilter>().mesh.vertices;

        foreach (var item in verticeList) {

            vertexList.Add( meshObject.transform.TransformPoint(item));
        }

        StartCoroutine("createObject");
    }

    IEnumerator createObject() {

        yield return new WaitForSeconds(waitTime);

        Instantiate(obj, vertexList[Random.Range(0, vertexList.Count)], Quaternion.identity);

        StartCoroutine("createObject");
    }

}
