using UnityEngine;

public class PhysicsBounce : MonoBehaviour
{    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has a Rigidbody component
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Calculate the bounce direction
            Vector3 bounceDirection = Vector3.Reflect(collision.relativeVelocity.normalized, collision.contacts[0].normal);
            // Apply an impulse force in the bounce direction
            rb.linearVelocity = bounceDirection * rb.linearVelocity.magnitude;
            Debug.Log("Bounced off " + collision.collider.name);
        }
    }
}
