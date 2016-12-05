using UnityEngine; 
using System.Collections;

public class Particle : MonoBehaviour {

    public Emitter emitter { get; set; }
    public Vector3 velocity { get; set; }
    
    public float rotationSpeed { get; set; }            // In angles
    public Vector3 rotationAxis { get; set; }

    public float distanceToKill { get; set; }
    public Vector3 pointOfEmission { get; set; }        // Point I spawned at

    void Start() {

        StartCoroutine("checkDistance");
    }

    void Update() {

        // Movement
        if (velocity != Vector3.zero)
            transform.position += velocity;

        // Rotation
        transform.Rotate(rotationAxis, rotationSpeed);

    }


    public void AddForce(Vector3 force) {

        velocity += force;
    }


    public void setRotation(float rotationSpeedMin, float rotationSpeedMax) {

        // Add random deviation to this
        rotationAxis = Vector3.right;
        rotationSpeed = Random.Range(rotationSpeedMin, rotationSpeedMax);
    }

    public void reset() {

        velocity = Vector3.zero;
        rotationSpeed = 0;
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
