using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))] //Auto adds rigidbody when script is added if none exists
public class CarController : MonoBehaviour
{
Rigidbody rb;
[SerializeField]
private float power = 5; //Speed of car
[SerializeField]
private float torque=0.5f; // Turning speed of car
[SerializeField]
private float maxSpeed =5; //speed cap


[SerializeField]
private Vector2 movementVector; //movement vector for car
private void Awake()
{
    rb = GetComponent<Rigidbody>();

    // drag and angular drag for stabilization--> somehow cuts down on jittering of the car except when entering a turn
    rb.drag = 1f;         // Controls forward/backward damping
    rb.angularDrag = 2f;  // Controls rotational damping
}


    // private void Awake(){
    //     rb=GetComponent<Rigidbody>();
    // }


    public void Move(Vector2 movementInput){
        this.movementVector=movementInput;
    }

    public void Stop(){
        this.movementVector=Vector2.zero;
    }
    
    private void FixedUpdate()
{
    if (rb.velocity.magnitude <= maxSpeed)
    {
        rb.AddForce(movementVector.y * transform.forward * power); // Forward movement
    }

    // 
    // rb.AddTorque(movementVector.x*Vector3.up*torque*movementVector.y);//turning

    // extra damping 
    // // Clamp the movement input's x-axis (turning input) for smoother turning
    float clampedX = Mathf.Clamp(movementVector.x, -1f, 1f);

    // // Add torque with a reduced multiplier for smoother turning
    rb.AddTorque(clampedX * Vector3.up * torque * movementVector.y);
}




}


