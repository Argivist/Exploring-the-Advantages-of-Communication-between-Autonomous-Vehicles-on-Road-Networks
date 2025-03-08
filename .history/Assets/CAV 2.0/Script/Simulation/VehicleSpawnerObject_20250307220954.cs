using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

public class VehicleSpawnerObject : MonoBehaviour
{
    [Header("Vehicle Object")]
    public GameObject vehicleObject;

    [Header("SpawnInfo")]
    public int id;
    public VehicleType type;

    public Segment startSegment;
    public Segment endSegment;

    [Header("Other")]
    public float spawnInterval; // may not be necessary

    void Start()
    {
        // Get near waypoint in start segment
        direction();
    }

    void direction()
    {
        float minDist = float.MaxValue;
        Waypoint closestWaypoint = null;

        // Find the nearest waypoint in front of the vehicle
        foreach (Waypoint w in startSegment.waypoints)
        {
            float d = Vector3.Distance(this.transform.position, w.transform.position);
            Vector3 localSpace = this.transform.InverseTransformPoint(w.transform.position);

            if (d < minDist && localSpace.z > 0) // Ensure it's in front
            {
                minDist = d;
                closestWaypoint = w;
            }
        }

        // Ensure we have a valid waypoint before proceeding
        if (closestWaypoint != null)
        {
            Debug.Log("Start Position: " + this.transform.position);
            Debug.Log("Waypoint Position: " + closestWaypoint.transform.position);

            // Compute direction
            Vector3 direction = (closestWaypoint.transform.position - this.transform.position).normalized;
            direction.y = 0; // Ensure we only rotate around Y-axis

            // Compute angle correctly
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Debug.Log("Computed Angle: " + angle);

            // Apply rotation
            this.transform.rotation = Quaternion.Euler(0, angle, 0);
        }
        else
        {
            Debug.LogWarning("No valid waypoint found in front!");
        }
    }
}
