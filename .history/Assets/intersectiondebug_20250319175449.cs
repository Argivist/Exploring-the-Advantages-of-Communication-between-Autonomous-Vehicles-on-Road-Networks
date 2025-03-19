using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
[System.Serializable]
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
    public List<ID> idList = new List<ID>();
    public List<VehicleDebug> vehiclesList = new List<VehicleDebug>();
    // Start is called before the first frame update
    void Start()
    {
                // ts=GameObject.Find("TrafficSimulation").GetComponent<TrafficSystem>();
                idList = new List<ID>();
                foreach(Intersection i in ts.intersections)
                {
                    List<VehicleDebug> vs = new List<VehicleDebug>();
                    vs=i.vehiclesList;
                    idList.Add(new ID(i,vs));
                }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
