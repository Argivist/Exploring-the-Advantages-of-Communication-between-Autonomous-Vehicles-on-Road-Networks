using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

[System.Serializable]
public class ID
{
    
    public List<VehicleDebug> vehicles;
    public List<GameObject> VehicleQueue;
    public List<GameObject> vehiclesInIntersection;

    
}

public class IntersectionDebug : MonoBehaviour
{
    public TrafficSystem ts;
    public List<VehicleDebug> vehicles;

    void Start()
    {      

        vehicles = gameObject.GetComponent<Intersection>().vehiclesList;
        VehicleQueue = gameObject.GetComponent<Intersection>().GetVehiclesQueue();
        vehiclesInIntersection = gameObject.GetComponent<Intersection>().GetVehiclesInIntersection();
    }
}
