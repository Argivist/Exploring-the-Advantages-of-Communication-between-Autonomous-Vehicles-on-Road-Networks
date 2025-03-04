using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Auto adds Rigidbody if not present
public class CarController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float power = 5f;  // Speed of car

    [SerializeField]
    private float torque = 0.5f;  // Turning speed

    [SerializeField]
    private float maxSpeed = 5f;  // Speed cap

    [SerializeField]
    private Vector2 movementVector;  // Movement vector for car

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Drag settings for stabilization
        rb.drag = 1f;         // Linear damping (reduces jittering)
        rb.angularDrag = 2f;  // Rotational damping
    }

    public void Move(Vector2 movementInput)
    {
        movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        // Apply forward force if speed is below the cap
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movementVector.y * transform.forward * power, ForceMode.Acceleration);
        }
        else
        {
            // If speed exceeds maxSpeed, limit it explicitly
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }

        // Apply turning torque (decoupled from forward motion)
        if (Mathf.Abs(movementVector.x) > 0.01f) // Avoid applying unnecessary torque when there's no input
        {
            rb.AddTorque(movementVector.x * Vector3.up * torque, ForceMode.Acceleration);
        }
    }
}
