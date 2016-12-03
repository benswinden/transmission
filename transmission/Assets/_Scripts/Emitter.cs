using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emitter : MonoBehaviour {

    public List<GameObject> meshObjectList;

    public List<GameObject> particleMaterials = new List<GameObject>();

    public float scale;

    [Space]

    public GameObject emissionMesh;

    [Space]

    public bool particleRandomStartRotation;
    public bool particleGravity;
    public float particleForce;    

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

        GameObject meshObjectPrefab = meshObjectList[Random.Range(0, meshObjectList.Count)];
        Vector3 vertex = vertexList[Random.Range(0, vertexList.Count)];

        GameObject newObj = Instantiate(meshObjectPrefab, vertex, Quaternion.identity) as GameObject;        

        bool found = false;
        foreach (Transform child in newObj.transform) {
            
            found = true;
        }

        if (!found) {

            var tempNewObject = new GameObject();
            newObj.transform.parent = tempNewObject.transform;            
            newObj = tempNewObject;
        }

        newObj.name = "Particle";
        newObj.AddComponent<Particle>();                

        newObj.GetComponent<Particle>().rigidbodyComponent = newObj.AddComponent<Rigidbody>();
        newObj.GetComponent<Particle>().rigidbodyComponent.useGravity = particleGravity;
        
        var collider = newObj.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        newObj.transform.localScale = new Vector3(scale, scale, scale);

        if (particleForce != 0) newObj.GetComponent<Particle>().rigidbodyComponent.AddForce(transform.up * particleForce, ForceMode.Impulse);
        if (particleRandomStartRotation) newObj.transform.rotation = Random.rotation;

    }
}
