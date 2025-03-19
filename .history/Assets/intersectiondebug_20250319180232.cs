using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;

[System.Serializable]
public class ID
{
    
    public List<VehicleDebug> vehicles;

    
}

public class IntersectionDebug : MonoBehaviour
{
    public TrafficSystem ts;
    public List<VehicleDebug> vehicles;

    void Start()
    {      

        vehicles = gameObject.GetComponent<Intersection>().vehiclesList;
    }
}
