using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {


    Rigidbody rigidbodyComponent;

    void Awake() {

        rigidbodyComponent = GetComponent<Rigidbody>();        
    }

    public void initialize(Mesh mesh, float particleForce, bool randomStartRotation) {

        GetComponentInChildren<MeshFilter>().mesh = mesh;
        GetComponentInChildren<MeshFilter>().gameObject.AddComponent<BoxCollider>();
        GetComponentInChildren<BoxCollider>().isTrigger = true;
        

        if (particleForce != 0) rigidbodyComponent.AddForce(transform.up * particleForce, ForceMode.Impulse);
        if (randomStartRotation) transform.rotation = Random.rotation;
    }

    void OnTriggerEnter(Collider other) {
        
        if (other.tag.Equals("Culler"))
            kill();
    }

    void kill() {

        Destroy(gameObject);
    }

}
