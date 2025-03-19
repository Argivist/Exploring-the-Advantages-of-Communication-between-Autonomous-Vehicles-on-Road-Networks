using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

[System.Serializable]
public class ID
{
    public Intersection i;
    public List<VehicleDebug> vehicles;

    public ID(Intersection i, List<VehicleDebug> vehicles)
    {
        this.i = i;
        this.vehicles = new List<VehicleDebug>(vehicles); // Ensures a new list is created
    }
}

public class IntersectionDebug : MonoBehaviour
{
    public TrafficSystem ts;
    public List<ID> idList = new List<ID>();

    void Start()
    {
        if (ts == null)
        {
            ts = FindObjectOfType<TrafficSystem>(); // Auto-assign if not set
        }

        if (ts == null || ts.intersections == null)
        {
            Debug.LogError("TrafficSystem or intersections list is null!");
            return;
        }

        foreach (Intersection i in ts.intersections)
        {
            // Ensure there's only one IntersectionDebug per Intersection
            if (i.GetComponent<IntersectionDebug>() == null)
            {
                IntersectionDebug d = i.gameObject.AddComponent<IntersectionDebug>();
                d.ts = ts; // Pass TrafficSystem reference

                // Create a deep copy of vehiclesList to avoid modifying the original list
                List<VehicleDebug> vs = new List<VehicleDebug>(i.vehiclesList);
                d.idList.Add(new ID(i, vs));
            }
        }
    }
}
