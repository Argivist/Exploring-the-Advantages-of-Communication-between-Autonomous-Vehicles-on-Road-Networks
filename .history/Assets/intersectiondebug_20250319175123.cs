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
    public TrafficSystem ts;
    public List<VehicleDebug> vehiclesList = new List<VehicleDebug>();
    public List<ID> idList = new List<ID>();
    // Start is called before the first frame update
    void Start()
    {
                ts=GameObject.Find("TrafficSimulation").GetComponent<TrafficSystem>();
                foreach(Intersection i in ts.intersections)
                {
                    List<VehicleDebug> vehicles = new List<VehicleDebug>();
                    foreach(VehicleDebug v in vehiclesList)
                    {
                        
                    }
                    idList.Add(new ID(i,vehicles));
                }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
