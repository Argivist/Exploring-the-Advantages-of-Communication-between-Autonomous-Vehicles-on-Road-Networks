using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Auto-adds Rigidbody if not present
public class CarController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float power = 5f; // Acceleration force
    [SerializeField] private float torque = 0.5f; // Turning force
    [SerializeField] private float maxSpeed = 5f; // Speed limit

    private Vector2 movementVector;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Stability settings to reduce jittering
        rb.drag = 1f;         // Linear damping
        rb.angularDrag = 2f;  // Rotational damping
    }

    public void Move(Vector2 movementInput)
    {
        this.movementVector = movementInput;
    }

    public void Stop()
    {
        this.movementVector = Vector2.zero;
        rb.velocity = Vector3.zero; // Ensure the car stops moving
        rb.angularVelocity = Vector3.zero; // Ensure the car stops rotating
    }

    private void FixedUpdate()
    {
        // Limit speed to maxSpeed
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            rb.AddForce(movementVector.y * transform.forward * power, ForceMode.Acceleration);
        }

        // Smoother turning
        float clampedX = Mathf.Clamp(movementVector.x, -1f, 1f);
        rb.AddTorque(clampedX * Vector3.up * torque, ForceMode.Acceleration);
    }
}
