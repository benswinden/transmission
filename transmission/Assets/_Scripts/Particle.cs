using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {

    public Rigidbody rigidbodyComponent { get; set; }


    public Emitter emitter { get; set; }

    void OnTriggerEnter(Collider other) {
        
        if (other.tag.Equals("Culler"))
            kill();
    }

    void kill() {

        emitter.objectDeactivated(gameObject);        
    }
}
