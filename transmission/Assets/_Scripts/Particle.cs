using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {


    Rigidbody rigidbody;

    void Awake() {

        rigidbody = GetComponent<Rigidbody>();        
    }

    public void initialize(Mesh mesh, float particleForce) {

        GetComponentInChildren<MeshFilter>().mesh = mesh;
        if (particleForce != 0) rigidbody.AddForce(transform.up * particleForce, ForceMode.Impulse);
    }

    void OnTriggerEnter(Collider other) {

        kill();
    }

    void kill() {

        Destroy(gameObject);
    }

}
