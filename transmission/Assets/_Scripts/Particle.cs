using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

public class Particle : MonoBehaviour {


    float dampTime = 0.15F;


    public Emitter emitter { get; set; }
    public Vector3 velocity { get; set; }    

    public float distanceToKill { get; set; }
    public Vector3 pointOfEmission { get; set; }        // Point I spawned at


    List<GameObject> meshList = new List<GameObject>();                  // Contains children
    List<Vector3> rotationAxisList = new List<Vector3>();        // a list for each different meshes rotation axis and speed
    List<float> rotationSpeedList = new List<float>();

    private Vector3 _velocity = Vector3.zero;


    void Awake() {

        foreach (Transform child in transform) {

            meshList.Add(child.gameObject);
            rotationAxisList.Add(Vector3.zero);
            rotationSpeedList.Add(0f);
        }
    }

    void Start() {

        StartCoroutine("checkDistance");
    }

    void FixedUpdate() {

        // Movement
        if (velocity != Vector3.zero) {

            var targetPos = transform.position += velocity;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, dampTime);
        }

        // Rotation
        for (int i = 0; i < meshList.Count; i++) {

            if (rotationAxisList.Count > i && rotationSpeedList.Count > i && rotationSpeedList[i] != 0)
                meshList[i].transform.Rotate(rotationAxisList[i], rotationSpeedList[i]);
        }        
    }

    public void AddForce(Vector3 force) {

        velocity += force;
    }


    public void setRotationValues(float rotationSpeedMin, float rotationSpeedMax) {
        
        for (int i = 0; i < meshList.Count; i++) {
            
            // Add random deviation to this
            rotationAxisList[i] = Vector3.right;
            rotationSpeedList[i] = Random.Range(rotationSpeedMin, rotationSpeedMax);
        }        
    }

    public void setRandomRotation() {

        foreach (GameObject mesh in meshList) 
            mesh.transform.rotation = Random.rotation;
    }

    public void setCameraFollow() {

        Manager.camera.setTarget(gameObject);
    }

    public void reset() {

        velocity = Vector3.zero;
        for (int i = 0; i < rotationSpeedList.Count; i++)
            rotationSpeedList[i] = 0;            
    }

    void kill() {

        emitter.objectDeactivated(gameObject);
    }

    IEnumerator checkDistance() {

        if (Vector3.Distance(pointOfEmission, transform.position) > distanceToKill)
            kill();

        yield return new WaitForSeconds(0.2f);

        StartCoroutine("checkDistance");
    }
}
