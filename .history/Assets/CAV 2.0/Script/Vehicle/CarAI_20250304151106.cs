using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarAI : MonoBehaviour
{
    [SerializeField] private List<Vector3> path = null;
    [SerializeField] private float arriveDistance = 0.3f;
    [SerializeField] private float lastPointArriveDistance = 0.1f;
    [SerializeField] private float turningAngleOffset = 5f;

    [SerializeField] private Vector3 currentTargetPosition;
    [SerializeField] private Navigation navigation;

    [SerializeField] private GameObject raycastStartingPoint;
    [SerializeField] private float collisionRaycastLength = 0.1f;

    private bool complete = false;
    private int index = 0;
    private bool collisionStop = false;
    private bool stop = false;

    public bool Stop
    {
        get { return stop || collisionStop; }
        set { stop = value; }
    }

    [field: SerializeField] public UnityEvent<Vector2> OnDrive { get; set; }

    private void Start()
    {
        navigation = GetComponent<Navigation>();

        if (path == null || path.Count == 0)
        {
            Stop = true;
        }
        else
        {
            currentTargetPosition = path[index];
        }
    }

    public void SetPath(List<Vector3> newPath)
    {
        if (newPath == null || newPath.Count == 0)
        {
            // complete = navigation.OntoNextSegment();
            if (complete)
            {
                Stop = true;
                Destroy(gameObject);
            }
            return;
        }

        path = newPath;
        index = 0;
        currentTargetPosition = path[index];

        // Rotate car to face the first point
        Vector3 relativePoint = transform.InverseTransformPoint(path[index + 1]);
        float angle = Mathf.Atan2(relativePoint.x, relativePoint.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, angle, 0);

        Stop = false;
    }

    private void Update()
    {
        CheckForCollisions();
        CheckIfArrived();
        Drive();
    }

    private void CheckForCollisions()
    {
        bool hitSomething = Physics.Raycast(
            raycastStartingPoint.transform.position,
            transform.forward,
            collisionRaycastLength,
            ~0 // Check against all layers
        );

        collisionStop = hitSomething;
    }

    private void CheckIfArrived()
    {
        if (!Stop)
        {
            float distanceToCheck = (index == path.Count - 1) ? lastPointArriveDistance : arriveDistance;

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
        {
            // complete = navigation.OntoNextSegment();
            if (complete)
            {
                Stop = true;
                Destroy(gameObject);
            }
            else
            {
                SetPath(path);
            }
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
            OnDrive?.Invoke(Vector2.zero);
        }
        else
        {
            Vector3 relativePoint = transform.InverseTransformPoint(currentTargetPosition);
            float angle = Mathf.Atan2(relativePoint.x, relativePoint.z) * Mathf.Rad2Deg;

            int rotateCar = 0;
            if (Mathf.Abs(angle) > turningAngleOffset)
            {
                rotateCar = Mathf.Sign(angle) > 0 ? 1 : -1;
            }

            OnDrive?.Invoke(new Vector2(rotateCar, 1));
        }
    }

    public void StopVehicle()
    {
        stop = true;
        OnDrive?.Invoke(Vector2.zero);
    }
}
