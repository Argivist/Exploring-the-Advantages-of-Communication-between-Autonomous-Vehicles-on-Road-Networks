using System.Collections;
using System.Collections.Generic;
using TrafficSimulation;
using UnityEngine;
using static Navigation;
[System.Serializable]
public class Vehicle
{
    //vehicle name, typ, start time, end time list depending on number o simulations
    public int vehicleId;
    public string vehicleName;


    public VehicleType vehicleType;
    public int startTime;
    public Vector3 startPos;
    public Vector3 endPos;
    public Waypoint wdir;
    public Segment destSegment;

    public Vehicle(int vehicle_id, string vehicleName, VehicleType vehicleType, int startTime, Vector3 startPos, Vector3 endPos, Waypoint wdir, Segment destSegment)
    {
        this.vehicleId = vehicle_id;
        this.vehicleName = vehicleName;
        this.vehicleType = vehicleType;
        this.startTime = startTime;
        this.startPos = startPos;
        this.endPos = endPos;
        this.wdir = wdir;
        this.destSegment = destSegment;

    }
}

[System.Serializable]
//waypoint with segment, waypoiint adn waypoint position
public class WaypointObject
{
    public Segment segment;
    public Waypoint currentWaypoint;
    public Waypoint nextWaypoint;
    public Waypoint previousWaypoint;
    public Vector3 position;

    public WaypointObject(Segment segment, Waypoint current, Waypoint next, Waypoint previous, Vector3 position)
    {
        this.segment = segment;
        this.currentWaypoint = current;
        this.nextWaypoint = next;
        this.previousWaypoint = previous;
        this.position = position;
    }
    public bool hasNext()
    {
        return nextWaypoint != null;
    }
    public bool hasPrevious()
    {
        return previousWaypoint != null;
    }

}


public class SimulationConfigurer : MonoBehaviour
{
    [Header("Simulation Configurer")]

    public int VehicleDensity;
    public bool ready = false;

    [Header("Traffic System")]
    public TrafficSystem trafficSystem;
    public List<Vehicle> vehicleList = new List<Vehicle>();






}
