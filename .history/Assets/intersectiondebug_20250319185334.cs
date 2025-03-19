using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

[System.Serializable]


public class IntersectionDebug : MonoBehaviour
{
    public TrafficSystem ts;
    public List<VehicleDebug> vehicles;

    public List<GameObject> VehicleQueue { get; private set; }
    public List<GameObject> VehiclesInIntersection { get; private set; }

    void Start()
    {      

        vehicles = gameObject.GetComponent<Intersection>().vehiclesList;
        VehicleQueue = gameObject.GetComponent<Intersection>().GetVehiclesQueue();
        VehiclesInIntersection = gameObject.GetComponent<Intersection>().GetVehiclesInIntersection();
    }
}
