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


    private void Awake(){
        rb=GetComponent<Rigidbody>();
    }


    public void Move(Vector2 movementInput){
        this.movementVector=movementInput;
    }

    
    private void FixedUpdate(){
        if(rb.velocity.magnitude<=maxSpeed){
            rb.AddForce(movementVector.y*transform.forward*power);//makes sure we can turn
        }
        rb.AddTorque(movementVector.x*Vector3.up*torque*movementVector.y);//turning
    }
}


