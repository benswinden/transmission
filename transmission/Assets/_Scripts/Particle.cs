using UnityEngine;
using System.Collections;

public class Particle : MonoBehaviour {

    public Rigidbody rigidbodyComponent { get; set; }    


    void OnTriggerEnter(Collider other) {
        
        if (other.tag.Equals("Culler"))
            kill();
    }

    void kill() {

        Destroy(gameObject);
    }

}
