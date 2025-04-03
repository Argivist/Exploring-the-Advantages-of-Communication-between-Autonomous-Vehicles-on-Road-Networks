using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

[System.Serializable]


public class IntersectionDebug : MonoBehaviour
{
    public TrafficSystem ts;
    public List<VehicleDebug> vehicles;

    public List<GameObject> VehicleQueue;
    public List<GameObject> VehiclesInIntersection;

    void Start()
    {      
        
        vehicles = gameObject.GetComponent<Intersection>().vehiclesList;
        VehicleQueue = gameObject.GetComponent<Intersection>().GetVehiclesQueue();
        VehiclesInIntersection = gameObject.GetComponent<Intersection>().GetVehiclesInIntersection();
    }
}
