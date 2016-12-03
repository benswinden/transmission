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
    public int maxPoolAmount = 100;

    [Space]    

    public GameObject emissionMesh;
    public bool hideDebugMeshes;

    [Space]

    public bool printDebug;


    List<Vector3> vertexList = new List<Vector3>();

    List<GameObject> createdObjectsActive = new List<GameObject>();
    List<GameObject> createdObjectsInactive = new List<GameObject>();

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

    void Update() {

        if (printDebug) Debug.Log("Object Pool > [Active: " + createdObjectsActive.Count + " | Inactive: " + createdObjectsInactive.Count + " ]");
    }

    IEnumerator instatiateLoop() {

        yield return new WaitForSeconds(waitTime);

        createObject();

        StartCoroutine("instatiateLoop");
    }


    void createObject() {
        
        // Check if we've filled the pool yet        
        if (createdObjectsActive.Count + createdObjectsInactive.Count < maxPoolAmount) {

            InstantiateNewObject();
        }
        else {

            activateNewObject();
        }
    }

    void InstantiateNewObject() {

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

        createdObjectsActive.Add(newObj);
        newObj.name = "Particle [ " + meshObjectPrefab.name + " ]";
        var particle = newObj.AddComponent<Particle>();     // Switched to referring to the particle component instead of the gameobject to save on a few calls .___.
        particle.emitter = this;

        // Materials
        foreach (GameObject obj in meshList) {
            obj.GetComponent<MeshRenderer>().material = particleMaterials[0];
        }

        // Transform
        particle.transform.localScale = new Vector3(scale, scale, scale);
        if (particleRandomStartRotation) particle.transform.rotation = Random.rotation;

        // Physics
        particle.rigidbodyComponent = newObj.AddComponent<Rigidbody>();
        particle.rigidbodyComponent.useGravity = particleGravity;

        // Force
        if (particleForce != 0) particle.rigidbodyComponent.AddForce(transform.up * particleForce, ForceMode.Impulse);

        var collider = particle.transform.GetChild(0).gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
    }

    void activateNewObject() {

        Vector3 vertex = vertexList[Random.Range(0, vertexList.Count)];

        if (createdObjectsInactive.Count == 0) { Debug.Log("Tried to activate a new object but there are no Inactive objects to use. Increase the object pool amount"); return; }

        var inactiveObjectIndex = Random.Range(0, createdObjectsInactive.Count);
        var particle = createdObjectsInactive[Random.Range(0, createdObjectsInactive.Count)].GetComponent<Particle>();

        createdObjectsInactive.Remove(particle.gameObject);
        createdObjectsActive.Add(particle.gameObject);

        particle.gameObject.SetActive(true);

        // Materials
        //foreach (GameObject obj in meshList) {
        //    obj.GetComponent<MeshRenderer>().material = particleMaterials[0];
        //}

        // Transform
        particle.transform.position = vertex;

        // Physics
        particle.rigidbodyComponent.useGravity = particleGravity;       // This may have changed since it was instatiated
        particle.rigidbodyComponent.velocity = Vector3.zero;             // Reset velocity

        if (particleForce != 0) particle.rigidbodyComponent.AddForce(transform.up * particleForce, ForceMode.Impulse);
    }

    public void objectDeactivated(GameObject obj) {

        createdObjectsActive.Remove(obj);
        createdObjectsInactive.Add(obj);
        obj.SetActive(false);
    }
}
