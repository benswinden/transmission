using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {


    public float force = 100;

    Rigidbody rigidbody;

    void Awake() {

        rigidbody = GetComponent<Rigidbody>();

        rigidbody.AddForce(transform.up * force, ForceMode.Impulse);
    }

}
