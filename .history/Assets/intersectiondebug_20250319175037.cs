using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
public class ID{
    Intersection i;
    List<VehicleDebug> vehicles;
    public ID(Intersection i, List<VehicleDebug> vehicles)
    {
        this.i = i;
        this.vehicles = vehicles;
    }
}
public class intersectiondebug : MonoBehaviour
{
    public TrafficSimulation ts;
    public List<VehicleDebug> vehiclesList = new List<VehicleDebug>();
    public List<ID> idList = new List<ID>();
    // Start is called before the first frame update
    void Start()
    {
                ts=GameObject.Find("TrafficSimulation").GetComponent<TrafficSimulation>();
                
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
