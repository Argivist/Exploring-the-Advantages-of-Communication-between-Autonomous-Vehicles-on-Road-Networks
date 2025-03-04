using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] //Auto adds Rigidbody when script is added if none exists
public class CarController : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float power = 5; //Speed of car
    [SerializeField] private float torque = 0.5f; // Turning speed of car
    [SerializeField] private float maxSpeed = 5; // Speed cap
    [SerializeField] private float detectionRange = 10f; // Distance to detect vehicles in front
    [SerializeField] private float stoppingDistance = 2f; // Minimum distance before full stop

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

    private void FixedUpdate()
    {
        float adjustedPower = power;

        // **Raycast to detect car in front**
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Car")) // Ensure other cars have the "Car" tag
            {
                float distance = hit.distance;

                // Reduce speed as we get closer to the car ahead
                if (distance < detectionRange)
                {
                    adjustedPower = Mathf.Lerp(0, power, (distance - stoppingDistance) / (detectionRange - stoppingDistance));
                }

                // If the car is too close, stop completely
                if (distance <= stoppingDistance)
                {
                    adjustedPower = 0;
                }
            }
        }

        // Apply forward force if speed is below maxSpeed
        if (rb.velocity.magnitude <= maxSpeed)
        {
            rb.AddForce(movementVector.y * transform.forward * adjustedPower);
        }

        // **Turning Logic (unchanged)**
        float clampedX = Mathf.Clamp(movementVector.x, -1f, 1f);
        rb.AddTorque(clampedX * Vector3.up * torque * movementVector.y);
    }

    // **Debugging Visualization**
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * detectionRange);
    }
}
