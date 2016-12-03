using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emitter : MonoBehaviour {

    public List<GameObject> meshObjectList;

    public List<Material> particleMaterials;

    [Space]

    public float scale;    

    [Space]

    public bool particleRandomStartRotation;
    public bool particleGravity;
    public float particleForce;    

    [Space]

    public float waitTime = 0.5f;

    [Space]    

    public GameObject emissionMesh;
    public bool hideDebugMeshes;

    List<Vector3> vertexList = new List<Vector3>();


    void Awake() {

        var verticeList = emissionMesh.GetComponent<MeshFilter>().mesh.vertices;

        foreach (var item in verticeList) {

            vertexList.Add(emissionMesh.transform.TransformPoint(item));
        }

        if (hideDebugMeshes) {
            emissionMesh.GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("Culler").GetComponent<MeshRenderer>().enabled = false;
            GameObject.Find("Focus Point (DOF)").GetComponent<MeshRenderer>().enabled = false;
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
        
        List<GameObject> meshList = new List<GameObject>();
        foreach (Transform child in newObj.transform) {

            meshList.Add(child.gameObject);
        }

        // If we didn't find any mesh children, that means we have a single mesh object so we need to create a parent for it
        if (meshList.Count == 0) {

            meshList.Add(newObj);

            var tempNewObject = new GameObject();
            tempNewObject.transform.position = vertex;
            newObj.transform.parent = tempNewObject.transform;            
            newObj = tempNewObject;            
        }

        newObj.name = "Particle [ " + meshObjectPrefab.name + " ]";
        newObj.AddComponent<Particle>();                

        // Materials
        foreach (GameObject obj in meshList) {
            obj.GetComponent<MeshRenderer>().material = particleMaterials[0];
        }

        // Physics
        newObj.GetComponent<Particle>().rigidbodyComponent = newObj.AddComponent<Rigidbody>();
        newObj.GetComponent<Particle>().rigidbodyComponent.useGravity = particleGravity;

            // Force
        if (particleForce != 0) newObj.GetComponent<Particle>().rigidbodyComponent.AddForce(transform.up * particleForce, ForceMode.Impulse);

        var collider = newObj.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;

        // Transform
        newObj.transform.localScale = new Vector3(scale, scale, scale);        
        if (particleRandomStartRotation) newObj.transform.rotation = Random.rotation;

    }
}
