using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float power = 5f; // Speed of car
    [SerializeField] private float torque = 0.5f; // Turning speed of car
    [SerializeField] private float maxSpeed = 5f; // Speed cap
    [SerializeField] private float dist;

    [SerializeField] private float detectionRange = 10f; // Distance to detect vehicles in front
    [SerializeField] private float stoppingDistance = 2f; // Minimum distance before full stop
    [SerializeField] private float brakingForce = 3f; // Braking intensity
    [SerializeField] private float angularDamping = 5f; // Rotation damping when stopping

    [SerializeField] private Vector2 movementVector; // Movement vector for car

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Drag and angular drag for stabilization
        rb.drag = 1f;         // Controls forward/backward damping
        rb.angularDrag = 2f;  // Controls rotational damping
    }

    public void Move(Vector2 movementInput)
    {
        this.movementVector = movementInput;
    }

    public void Stop()
    {
        this.movementVector = Vector2.zero;
    }

    private void FixedUpdate()
    {
        float adjustedPower = power;
        bool shouldBrake = false;

        // **Raycast to detect car in front**
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Car")) // Ensure other cars have the "Car" tag
            {
                float distance = hit.distance;
                dist = distance;

                // Reduce speed progressively
                if (distance < detectionRange)
                {
                    adjustedPower = Mathf.Lerp(0, power, (distance - stoppingDistance) / (detectionRange - stoppingDistance));
                }

                // If too close, apply braking force
                if (distance <= stoppingDistance)
                {
                    adjustedPower = 0;
                    shouldBrake = true;
                }
            }
        }

        // **Apply Braking & Stop Rotating**
        if (shouldBrake)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, brakingForce * Time.fixedDeltaTime);
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, angularDamping * Time.fixedDeltaTime);
        }
        else
        {
            // Apply forward force only if below max speed
            if (rb.velocity.magnitude <= maxSpeed)
            {
                rb.AddForce(movementVector.y * transform.forward * adjustedPower);
            }
        }

        // **Turn Only When Moving Forward**
        if (rb.velocity.magnitude > 0.1f)
        {
            float clampedX = Mathf.Clamp(movementVector.x, -1f, 1f);
            float dynamicTorque = torque * (1f - Mathf.Clamp01(rb.velocity.magnitude / maxSpeed)); // Reduce turn at high speed
            rb.AddTorque(clampedX * Vector3.up * dynamicTorque * movementVector.y);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * detectionRange);
    }
}
