using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class CarAI : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> path = null;
    [SerializeField]
    private float arriveDistance = 0.3f, lastPointArriveDistance = 0.1f;// Arrive distance: how close the car should be to the point before moving to the next point, lastPointArriveDistance: how close the car should be to the last point before stopping;
                                                                        // Arrive distance is usually larger than lastPointArriveDistance because when turnning we want a larger margin of eerror than when we are stopping
    [SerializeField]
    private float turningAngleOffset = 5;// rotation of car with respect to point
    [SerializeField]
    private Vector3 currentTargetPosition;

    private int index = 0;// index of the current point in the path

    private bool stop;
    public bool Stop
    {
        get { return stop; }
        set { stop = value; }
    }

    [field: SerializeField]
    public UnityEvent<Vector2> OnDrive { get; set; }



    private void Start()
    {
        if (path == null || path.Count == 0)
        {
            Stop = true;
        }
        else
        {
            currentTargetPosition = path[index];

        }
    }


    //set path for car

    public void SetPath(List<Vector3> path)
    {
        //if no further path or reached the end of the path
        if (path.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        this.path = path;
        index = 0;
        currentTargetPosition = this.path[index];

        Vector3 relativepoint = transform.InverseTransformPoint(this.path[index + 1]);//relative position from car to point

        float angle = Mathf.Atan2(relativepoint.x, relativepoint.z) * Mathf.Rad2Deg; //angle between car and point in degree

        // to face point
        transform.rotation = Quaternion.Euler(0, angle, 0);

        Stop = false;//start car to move o next point
    }


    private void Update()
    {
        CheckIfArrived();
        Drive();
    }


    private void CheckIfArrived()
    {
        if (Stop == false)
        {
            var distanceToCheck = arriveDistance;
            if (index == path.Count - 1)
            {//if last distance
                distanceToCheck = lastPointArriveDistance;
            }
            if (Vector3.Distance(currentTargetPosition, transform.position) < distanceToCheck)
            {
                SetNextTarget();
            }
        }
    }



    private void SetNextTarget()
    {
        index++;
        if (index >= path.Count)
        {//outside of bounds of list of positions
            Stop = true;
            Destroy(gameObject);
            return;
        }
        else
        {
            currentTargetPosition = path[index];
        }
    }

    private void Drive()
    {
        if (Stop)
        {
            OnDrive?.Invoke(Vector2.zero);//checks if anyone is lstening if a method has been assigned to the event
        }
        else
        {
            Vector3 relativepoint = transform.InverseTransformPoint(currentTargetPosition);//relative position from car to point

            float angle = Mathf.Atan2(relativepoint.x, relativepoint.z) * Mathf.Rad2Deg; //angle between car and point in degree

            var rotateCar = 0;
            if (angle > turningAngleOffset)
            {
                rotateCar = 1;
            }
            else if (angle < -turningAngleOffset)
            {
                rotateCar = -1;
            }
            OnDrive?.Invoke(new Vector2(rotateCar, 1));


        }
    }
}
