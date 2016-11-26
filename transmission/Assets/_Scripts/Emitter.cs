﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emitter : MonoBehaviour {

    public List<Mesh> meshList;
    public GameObject particlePrefab;

    [Space]

    public GameObject emissionMesh;

    [Space]

    public float particleForce;
    public bool randomParticleStartRotation;

    [Space]

    public float waitTime = 0.5f;

    [Space]
    
    public bool hideEmissionMesh;

    List<Vector3> vertexList = new List<Vector3>();


    void Awake() {

        var verticeList = emissionMesh.GetComponent<MeshFilter>().mesh.vertices;

        foreach (var item in verticeList) {

            vertexList.Add(emissionMesh.transform.TransformPoint(item));
        }

        if (hideEmissionMesh) {
            emissionMesh.GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("Culler").GetComponent<MeshRenderer>().enabled = false;
        }

        StartCoroutine("instatiateLoop");
    }

    IEnumerator instatiateLoop() {

        yield return new WaitForSeconds(waitTime);

        createObject();

        StartCoroutine("instatiateLoop");
    }

    void createObject() {
        
        GameObject obj = Instantiate(particlePrefab, vertexList[Random.Range(0, vertexList.Count)], Quaternion.identity) as GameObject;
        obj.GetComponent<Particle>().initialize(meshList[Random.Range(0, meshList.Count)], particleForce, randomParticleStartRotation);
    }
}
