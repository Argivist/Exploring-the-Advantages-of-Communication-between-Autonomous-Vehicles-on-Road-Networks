using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Auto-adds Rigidbody if not present
public class CarController : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float power = 5f;  // Acceleration force
    [SerializeField] private float torque = 0.5f;  // Base turning force
    [SerializeField] private float maxSpeed = 5f;  // Speed cap
    [SerializeField] private float turnSmoothing = 5f;  // Higher values make turning smoother
    [SerializeField] private float turnReductionAtSpeed = 0.5f; // Reduces turning power at high speed

    private float currentTurnForce = 0f; // Smoothly adjusted turn force
    private Vector2 movementVector;  // Input vector

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Stabilization settings to reduce jittering
        rb.drag = 1f;         // Forward/backward damping
        rb.angularDrag = 2f;  // Rotational damping
    }

    public void Move(Vector2 movementInput)
    {
        movementVector = movementInput;
    }

    private void FixedUpdate()
    {
        // Apply forward force if speed is below maxSpeed
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(movementVector.y * transform.forward * power, ForceMode.Acceleration);
        }
        else
        {
            // Clamp velocity if it exceeds maxSpeed
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
        }

        // **Smoothed Turning**
        float clampedX = Mathf.Clamp(movementVector.x, -1f, 1f);

        // Reduce turn strength slightly based on speed (high speed = lower turn impact)
        float speedFactor = Mathf.Lerp(1f, turnReductionAtSpeed, rb.velocity.magnitude / maxSpeed);
        float targetTurnForce = clampedX * torque * speedFactor;

        // Smoothly transition the turn force to avoid jerky motion
        currentTurnForce = Mathf.Lerp(currentTurnForce, targetTurnForce, Time.fixedDeltaTime * turnSmoothing);

        // Apply the smoothly adjusted torque
        rb.AddTorque(currentTurnForce * Vector3.up, ForceMode.Acceleration);
    }
}
