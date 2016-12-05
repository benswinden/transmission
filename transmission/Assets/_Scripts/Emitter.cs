using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emitter : MonoBehaviour {

    public List<GameObject> meshObjectList;

    //public List<Material> particleMaterials;

    [Space]

    public float scaleMin;    
    public float scaleMax;    

    [Space]

    public bool randomStartRotation;
    public float distanceToKill;
    //public bool particleGravity;
    
    [Space]

    public float forceMin;    
    public float forceMax;

    [Space]

    public float rotationSpeedMin;
    public float rotationSpeedMax;

    [Space]

    public float createWaitMin;     // Wait before instatiation/activating the next object
    public float createWaitMax;

    public int maxPoolAmount;

    [Space]    

    public GameObject emissionGrid;
    public bool hideDebugMeshes;

    [Space]

    public bool printDebug;


    GameObject particleParent;


    List<Vector3> emissionPointList = new List<Vector3>();

    List<GameObject> createdObjectsActive = new List<GameObject>();
    List<GameObject> createdObjectsInactive = new List<GameObject>();

    void Awake() {

        foreach (Transform child in emissionGrid.transform) {

            emissionPointList.Add(child.position);

            if (hideDebugMeshes) child.GetComponent<MeshRenderer>().enabled = false;
        }


        if (hideDebugMeshes) {
            
            GameObject.Find("Focus Point (DOF)").GetComponent<MeshRenderer>().enabled = false;
        }

        particleParent = new GameObject();
        particleParent.name = "Particle Parent";
        particleParent.transform.parent = transform;

        StartCoroutine("instatiateLoop");
    }

    void Update() {

        if (printDebug) Debug.Log("Object Pool > [Active: " + createdObjectsActive.Count + " | Inactive: " + createdObjectsInactive.Count + " ]");
    }

    IEnumerator instatiateLoop() {

        var waitTime = Random.Range(createWaitMin, createWaitMax);
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
        Vector3 emissionPoint = emissionPointList[Random.Range(0, emissionPointList.Count)];            
        
        GameObject newObj = Instantiate(meshObjectPrefab, emissionPoint, Quaternion.identity) as GameObject;        

        List<GameObject> meshList = new List<GameObject>();
        foreach (Transform child in newObj.transform) {

            meshList.Add(child.gameObject);
        }

        // If we didn't find any mesh children, that means we have a single mesh object so we need to create a parent for it
        if (meshList.Count == 0) {

            meshList.Add(newObj);

            var tempNewObject = new GameObject();
            tempNewObject.transform.position = emissionPoint;
            newObj.transform.parent = tempNewObject.transform;
            newObj = tempNewObject;            
        }

        createdObjectsActive.Add(newObj);
        newObj.name = "Particle [ " + meshObjectPrefab.name + " ]";
        var particle = newObj.AddComponent<Particle>();     // Switched to referring to the particle component instead of the gameobject to save on a few calls .___.
        
        // Variables
        particle.emitter = this;
        particle.distanceToKill = distanceToKill;
        particle.pointOfEmission = emissionPoint;


        // Materials
        //foreach (GameObject obj in meshList) {

        //    var importedMaterialName = obj.GetComponent<MeshRenderer>().material.name;
        //    importedMaterialName = importedMaterialName.Substring(0, importedMaterialName.Length - 11);

        //    Material materialToUse = particleMaterials[0];
        //    foreach (Material mat in particleMaterials) {

        //        if (importedMaterialName.Equals(mat.name)) {

        //            materialToUse = mat;
        //            break;
        //        }
        //    }

        //    obj.GetComponent<MeshRenderer>().material = materialToUse;
        //}

        // Transform
        particle.transform.parent = particleParent.transform;
        var scale = Random.Range(scaleMin, scaleMax);
        particle.transform.localScale = new Vector3(scale, scale, scale);
        if (randomStartRotation) particle.setRandomRotation();
       

        // Force
        if (forceMax != 0) particle.AddForce(transform.up * Random.Range(forceMin, forceMax));

        if (rotationSpeedMax != 0) particle.setRotationValues(rotationSpeedMin, rotationSpeedMax);

    }

    void activateNewObject() {

        Vector3 emissionPoint = emissionPointList[Random.Range(0, emissionPointList.Count)];

        if (createdObjectsInactive.Count == 0) { Debug.Log("Tried to activate a new object but there are no Inactive objects to use. Increase the object pool amount"); return; }

        var inactiveObjectIndex = Random.Range(0, createdObjectsInactive.Count);
        var particle = createdObjectsInactive[Random.Range(0, createdObjectsInactive.Count)].GetComponent<Particle>();

        createdObjectsInactive.Remove(particle.gameObject);
        createdObjectsActive.Add(particle.gameObject);

        particle.gameObject.SetActive(true);

        particle.reset();

        // Variables        
        particle.distanceToKill = distanceToKill;
        particle.pointOfEmission = emissionPoint;

        // Materials
        //foreach (GameObject obj in meshList) {
        //    obj.GetComponent<MeshRenderer>().material = particleMaterials[0];
        //}

        // Transform
        particle.transform.position = emissionPoint;
        var scale = Random.Range(scaleMin, scaleMax);
        particle.transform.localScale = new Vector3(scale, scale, scale);
        if (randomStartRotation) particle.setRandomRotation();

        // Force                
        if (rotationSpeedMax != 0) particle.setRotationValues(rotationSpeedMin, rotationSpeedMax);
        if (forceMax != 0) particle.AddForce(transform.forward * Random.Range(forceMin, forceMax));
    }

    public void objectDeactivated(GameObject obj) {

        createdObjectsActive.Remove(obj);
        createdObjectsInactive.Add(obj);
        obj.SetActive(false);
    }
}
